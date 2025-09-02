import json
import os
import sys
import time
import subprocess
from typing import Tuple

import pika


# RabbitMQ connection (host = localhost if using Docker Desktop)
RABBITMQ_HOST = os.environ.get("RABBITMQ_HOST", "localhost")
RABBITMQ_USER = os.environ.get("RABBITMQ_USER", os.environ.get("ADMIN_USER_NAME", "root"))
RABBITMQ_PASS = os.environ.get("RABBITMQ_PASS", os.environ.get("ADMIN_PASS", "adminpass"))
QUEUE = os.environ.get("TTS_OUTPUT_QUEUE", "dot.speech.tts.response")
PREFETCH = int(os.environ.get("PREFETCH", "1"))

# Path mapping: container -> host
CONTAINER_AUDIO_PREFIX = os.environ.get("CONTAINER_AUDIO_PREFIX", "/shared/audio")
HOST_AUDIO_DIR = os.environ.get("HOST_AUDIO_DIR", os.path.abspath(os.path.join(os.getcwd(), "shared_audio")))


def ensure_host_dir():
    try:
        os.makedirs(HOST_AUDIO_DIR, exist_ok=True)
    except Exception:
        pass


def connect_rabbitmq(retries: int = 30, delay: float = 2.0):
    credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASS)
    params = pika.ConnectionParameters(host=RABBITMQ_HOST, credentials=credentials)
    attempt = 0
    while True:
        try:
            conn = pika.BlockingConnection(params)
            ch = conn.channel()
            ch.queue_declare(queue=QUEUE)
            ch.basic_qos(prefetch_count=PREFETCH)
            print(f"[HOST] Connected to RabbitMQ at {RABBITMQ_HOST}, queue '{QUEUE}'")
            return conn, ch
        except Exception as e:
            attempt += 1
            if attempt > retries:
                raise
            print(f"[HOST][WARN] RabbitMQ not ready (attempt {attempt}/{retries}): {e}")
            time.sleep(delay)


def parse_message(body: bytes) -> str:
    # JSON: {"path": "/shared/audio/....wav"} or plain text path
    try:
        payload = json.loads(body)
        if isinstance(payload, dict) and "path" in payload:
            return str(payload["path"]).strip()
    except Exception:
        pass
    # Fallback to text
    try:
        txt = body.decode("utf-8", errors="ignore").strip()
        if txt:
            return txt
    except Exception:
        pass
    raise ValueError("Unsupported message format: expected JSON with 'path' or a plain path")


def map_to_host_path(path: str) -> str:
    # If path is inside the container mount, map to HOST_AUDIO_DIR
    norm = path.replace("\\", "/")
    prefix = CONTAINER_AUDIO_PREFIX.rstrip("/")
    if norm.startswith(prefix + "/"):
        suffix = norm[len(prefix) + 1 :]
        host_path = os.path.join(HOST_AUDIO_DIR, *suffix.split("/"))
        return os.path.abspath(host_path)
    # If already a host path or relative path
    return os.path.abspath(path)


def play_windows(path: str) -> bool:
    try:
        import winsound

        print(f"[HOST] Playing (winsound): {path}")
        winsound.PlaySound(path, winsound.SND_FILENAME)
        return True
    except Exception as e:
        print(f"[HOST][WARN] winsound failed: {e}")
        return False


def run_cmd(cmd: list[str]) -> bool:
    try:
        print("[HOST] Exec:", " ".join(cmd))
        subprocess.run(cmd, check=True)
        return True
    except FileNotFoundError:
        return False
    except subprocess.CalledProcessError as e:
        print(f"[HOST][WARN] Command failed: {e}")
        return False


def play_macos(path: str) -> bool:
    return run_cmd(["afplay", path]) or run_cmd(["open", path])


def play_linux(path: str) -> bool:
    return (
        run_cmd(["ffplay", "-nodisp", "-autoexit", "-loglevel", "error", path])
        or run_cmd(["paplay", path])
        or run_cmd(["aplay", path])
        or run_cmd(["xdg-open", path])
    )


def play_audio(path: str):
    host_path = map_to_host_path(path)
    if not os.path.exists(host_path):
        print(f"[HOST][ERROR] File not found on host: {host_path}")
        return

    plat = sys.platform
    ok = False
    if plat.startswith("win"):
        ok = play_windows(host_path)
    elif plat == "darwin":
        ok = play_macos(host_path)
    else:
        ok = play_linux(host_path)

    if not ok:
        print(f"[HOST][INFO] Could not auto-play. File ready at: {host_path}")


def callback(ch, method, properties, body):
    print("[HOST] Message received")
    try:
        path = parse_message(body)
        play_audio(path)
        ch.basic_ack(delivery_tag=method.delivery_tag)
    except Exception as e:
        preview = (body[:120] if isinstance(body, (bytes, bytearray)) else str(body))
        print(f"[HOST][ERROR] Failed to process: {e}. Body preview: {preview}")
        ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)


def main():
    print("[HOST] dot_speech_player_host starting…")
    print(f"[HOST] Host audio dir: {HOST_AUDIO_DIR}")
    ensure_host_dir()
    conn, ch = connect_rabbitmq()
    print(f"[HOST] Waiting for messages on '{QUEUE}'…")
    ch.basic_consume(queue=QUEUE, on_message_callback=callback)
    ch.start_consuming()


if __name__ == "__main__":
    main()

