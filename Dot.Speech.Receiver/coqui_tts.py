from safetensors.torch import load_file
from TTS.api import TTS
import sys

text = sys.argv[1]
speaker_name = sys.argv[2]

# Load the XTTS model (v2 compatible)
tts = TTS(
    model_path="models/xtts",
    config_path="models/xtts/config.json",
    gpu=True
)

embedding_tensor = load_file(f"voices/{speaker_name}.safetensors")

tts.synthesizer.tts_model.speaker_manager.speakers[speaker_name] = {
    "gpt_cond_latent": embedding_tensor["gpt_cond_latent"],
    "speaker_embedding": embedding_tensor["speaker_embedding"]
}

output_path = "ai-response.wav"
tts.tts_to_file(
    text=text,
    speaker=speaker_name,
    language="en",
    speed=2.5,
    temperature=0.8,
    length_penalty=1.0, 
    file_path=output_path
)

print(f"Generated: {output_path}")