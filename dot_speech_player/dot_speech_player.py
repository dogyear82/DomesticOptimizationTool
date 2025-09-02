import os
import json
import time
import shlex
import subprocess
from typing import Tuple

import pika


RABBITMQ_HOST = os.environ.get("RABBITMQ_HOST", "localhost")
RABBITMQ_USER = os.environ.get("RABBITMQ_USER", "dot-app")
RABBITMQ_PASS = os.environ.get("RABBITMQ_PASS", "1LsQWPaEOXo1")
INPUT_QUEUE = os.environ.get("TTS_OUTPUT_QUEUE", "dot.speech.tts.response")
SHARED_VOLUME_PATH = os.environ.get("SHARED_VOLUME_PATH", "/shared/audio")
PREFETCH = int(os.environ.get("PREFETCH", "1"))
PLAYER_USE_FFPLAY = os.environ.get("PLAYER_USE_FFPLAY", "0").lower() in ("1","true","yes")


def connect_rabbitmq(retries: int = 30, delay: float = 2.0):
    credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASS)
    params = pika.ConnectionParameters(host=RABBITMQ_HOST, credentials=credentials)
    attempt = 0
    while True:
        try:
            conn = pika.BlockingConnection(params)
            ch = conn.channel()
            ch.queue_declare(queue=INPUT_QUEUE)
            ch.basic_qos(prefetch_count=PREFETCH)
            print("[INFO] Player connected to RabbitMQ")
            return conn, ch
        except Exception as e:
            attempt += 1
            if attempt > retries:
                raise
            print(f"[WARN] RabbitMQ not ready for player (attempt {attempt}/{retries}): {e}")
            time.sleep(delay)


def parse_message(body: bytes) -> str:
    # Expect {"path": "/shared/audio/..wav"}. Accept plain path as fallback
    try:
        payload = json.loads(body)
        if isinstance(payload, dict) and "path" in payload:
            return str(payload["path"]).strip()
    except Exception:
        pass
    # Try utf-8 text as a path
    try:
        txt = body.decode("utf-8", errors="ignore").strip()
        if txt:
            return txt
    except Exception:
        pass
    raise ValueError("Unsupported message format for player")


def resolve_path(p: str) -> str:
    if os.path.isabs(p):
        return p
    return os.path.join(SHARED_VOLUME_PATH, p)


def try_ffplay(path: str) -> bool:
    cmd = [
        "ffplay",
        "-nodisp",
        "-autoexit",
        "-loglevel",
        "error",
        path,
    ]
    try:
        print(f"[INFO] Playing with ffplay: {shlex.join(cmd)}")
        subprocess.run(cmd, check=True)
        return True
    except FileNotFoundError:
        print("[WARN] ffplay not found in container. Skipping playback.")
        return False
    except subprocess.CalledProcessError as e:
        print(f"[WARN] ffplay failed: {e}")
        return False


def play_audio(path: str):
    if not os.path.isabs(path):
        path = resolve_path(path)
    if not os.path.exists(path):
        raise FileNotFoundError(f"Audio file not found: {path}")
    if PLAYER_USE_FFPLAY:
        ok = try_ffplay(path)
        if not ok:
            print(f"[INFO] Playback skipped. File ready at: {path}")
    else:
        print(f"[INFO] PLAYER_USE_FFPLAY disabled. File ready at: {path}")


def callback(ch, method, properties, body):
    print("[INFO] Player received message")
    try:
        path = parse_message(body)
        play_audio(path)
        ch.basic_ack(delivery_tag=method.delivery_tag)
    except Exception as e:
        preview = (body[:120] if isinstance(body, (bytes, bytearray)) else str(body))
        print(f"[ERROR] Player failed to process message: {e}. Body preview: {preview}")
        # Do not requeue to avoid infinite loops on malformed messages
        ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)


def main():
    print("[INFO] Starting dot_speech_player...")
    conn, ch = connect_rabbitmq()
    print(f"[INFO] Waiting for messages on '{INPUT_QUEUE}'...")
    ch.basic_consume(queue=INPUT_QUEUE, on_message_callback=callback)
    ch.start_consuming()


if __name__ == "__main__":
    main()
