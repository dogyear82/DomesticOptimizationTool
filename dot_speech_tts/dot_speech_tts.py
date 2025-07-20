import os
import pika
import json
import uuid
import re
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
MODEL_PATH = "models/xtts"
CONFIG_PATH = "models/xtts/config.json"

# Load XTTS
print("[INFO] Loading XTTS model...")
tts = TTS(model_path=MODEL_PATH, config_path=CONFIG_PATH, gpu=True)
print("[INFO] Model loaded.")

# Connect to RabbitMQ
credentials = pika.PlainCredentials(RABBITMQ_USER, RABBITMQ_PASS)
params = pika.ConnectionParameters(host=RABBITMQ_HOST, credentials=credentials)
connection = pika.BlockingConnection(params)
channel = connection.channel()
channel.queue_declare(queue=INPUT_QUEUE)
channel.queue_declare(queue=OUTPUT_QUEUE)


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


def callback(ch, method, properties, body):
    print("[INFO] Message received")
    try:
        payload = json.loads(body)
        text = payload["text"]
        speaker = payload.get("speaker", "default")

        paths = generate_audio_segments(text, speaker)

        for p in paths:
            result_msg = json.dumps({"path": p})
            channel.basic_publish(exchange='', routing_key=OUTPUT_QUEUE, body=result_msg)
            print(f"[INFO] Sent file path to queue: {p}")

        ch.basic_ack(delivery_tag=method.delivery_tag)
    except Exception as e:
        print(f"[ERROR] Failed to process message: {e}")
        ch.basic_nack(delivery_tag=method.delivery_tag)


print("[INFO] Waiting for messages...")
channel.basic_consume(queue=INPUT_QUEUE, on_message_callback=callback)
channel.start_consuming()
