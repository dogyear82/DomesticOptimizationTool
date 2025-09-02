import os
import time
import pika
import json
import uuid
import re
import torch
from typing import Tuple
from safetensors.torch import load_file
from TTS.api import TTS

# Config
RABBITMQ_HOST = os.environ.get("RABBITMQ_HOST", "localhost")
RABBITMQ_PORT  = os.environ.get("RABBITMQ_PORT", "5672")
RABBITMQ_USER = os.environ.get("RABBITMQ_USER", "dot-app")
RABBITMQ_PASS = os.environ.get("RABBITMQ_PASS", "1LsQWPaEOXo1")
INPUT_QUEUE = os.environ.get("TTS_INPUT_QUEUE", "dot.speech.tts")
OUTPUT_QUEUE = os.environ.get("TTS_OUTPUT_QUEUE", "dot.speech.tts.response")
SHARED_VOLUME_PATH = os.environ.get("SHARED_VOLUME_PATH", "/shared/audio")
DEFAULT_SPEAKER = os.environ.get("TTS_DEFAULT_SPEAKER", "aloy")
MODEL_PATH = "models/xtts"
CONFIG_PATH = "models/xtts/config.json"

use_gpu_env = os.environ.get("TTS_USE_GPU", "auto").lower()
cuda_available = torch.cuda.is_available()
if use_gpu_env in ("1", "true", "yes"): 
    use_gpu = cuda_available
elif use_gpu_env in ("0", "false", "no"):
    use_gpu = False
else:
    use_gpu = cuda_available

# Load XTTS
print(f"[INFO] Loading XTTS model (gpu={use_gpu})...")
tts = TTS(model_path=MODEL_PATH, config_path=CONFIG_PATH, gpu=use_gpu)
print("[INFO] Model loaded.")

def connect_rabbitmq(retries=30, delay=2):
    credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASS)
    params = pika.ConnectionParameters(host=RABBITMQ_HOST, credentials=credentials)
    attempt = 0
    while True:
        try:
            conn = pika.BlockingConnection(params)
            ch = conn.channel()
            ch.queue_declare(queue=INPUT_QUEUE)
            ch.queue_declare(queue=OUTPUT_QUEUE)
            print("[INFO] Connected to RabbitMQ")
            return conn, ch
        except Exception as e:
            attempt += 1
            if attempt > retries:
                raise
            print(f"[WARN] RabbitMQ not ready (attempt {attempt}/{retries}): {e}")
            time.sleep(delay)

connection, channel = connect_rabbitmq()


def split_text(text):
    # Break on punctuation, keep it with the sentence
    segments = re.split(r'(?<=[.!?])\s+', text)
    return [s.strip() for s in segments if s.strip()]


def generate_audio_segments(text, speaker_name):
    # Load voice embedding
    print(f"[INFO] Loading voice embedding for {speaker_name}")
    embedding_tensor = load_file(f"voices/{speaker_name}.safetensors")
    tts.synthesizer.tts_model.speaker_manager.speakers[speaker_name] = {
        "gpt_cond_latent": embedding_tensor["gpt_cond_latent"],
        "speaker_embedding": embedding_tensor["speaker_embedding"]
    }

    segments = split_text(text)
    audio_paths = []

    for idx, segment in enumerate(segments):
        file_id = str(uuid.uuid4())
        file_path = os.path.join(SHARED_VOLUME_PATH, f"{file_id}.wav")

        print(f"[INFO] Generating segment {idx + 1}/{len(segments)}: '{segment}'")
        os.makedirs(os.path.dirname(file_path), exist_ok=True)
        tts.tts_to_file(
            text=segment,
            speaker=speaker_name,
            language="en",
            speed=2.5,
            temperature=0.8,
            length_penalty=1.0,
            file_path=file_path
        )

        audio_paths.append(file_path)
        print(f"[INFO] Wrote: {file_path}")

    return audio_paths


def resolve_speaker(requested: str) -> str:
    speaker_file = f"voices/{requested}.safetensors"
    if os.path.isfile(speaker_file):
        return requested
    # fallback to default
    if os.path.isfile(f"voices/{DEFAULT_SPEAKER}.safetensors"):
        print(f"[WARN] Speaker '{requested}' not found. Falling back to '{DEFAULT_SPEAKER}'.")
        return DEFAULT_SPEAKER
    # fallback to first available
    for fn in os.listdir("voices"):
        if fn.endswith(".safetensors"):
            spk = os.path.splitext(fn)[0]
            print(f"[WARN] Speaker '{requested}' not found. Using '{spk}'.")
            return spk
    raise FileNotFoundError("No speaker embeddings found in voices/ directory")


def parse_message(body: bytes) -> Tuple[str, str]:
    # Try JSON first
    try:
        payload = json.loads(body)
        if isinstance(payload, dict):
            text = payload.get("text")
            speaker = payload.get("speaker", DEFAULT_SPEAKER)
            if not text:
                raise ValueError("Missing 'text' in payload")
            return text, speaker
    except Exception:
        pass

    # Try as UTF-8 plain text
    try:
        txt = body.decode("utf-8", errors="ignore").strip()
        # If looks like JSON after decode, attempt once more
        if (txt.startswith("{") and txt.endswith("}")) or (txt.startswith("[") and txt.endswith("]")):
            try:
                payload = json.loads(txt)
                if isinstance(payload, dict) and "text" in payload:
                    return payload["text"], payload.get("speaker", DEFAULT_SPEAKER)
            except Exception:
                pass
        if txt:
            return txt, DEFAULT_SPEAKER
    except Exception:
        pass
    raise ValueError("Unsupported message format")


def callback(ch, method, properties, body):
    print("[INFO] Message received")
    try:
        text, speaker_req = parse_message(body)
        speaker = resolve_speaker(speaker_req)

        paths = generate_audio_segments(text, speaker)

        for p in paths:
            result_msg = json.dumps({"path": p})
            channel.basic_publish(exchange='', routing_key=OUTPUT_QUEUE, body=result_msg)
            print(f"[INFO] Sent file path to queue: {p}")

        ch.basic_ack(delivery_tag=method.delivery_tag)
    except Exception as e:
        preview = (body[:120] if isinstance(body, (bytes, bytearray)) else str(body))
        print(f"[ERROR] Failed to process message: {e}. Body preview: {preview}")
        # Do not requeue malformed messages to avoid infinite loop
        ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)


print("[INFO] Waiting for messages...")
channel.basic_consume(queue=INPUT_QUEUE, on_message_callback=callback)
channel.start_consuming()
