Dot Speech TTS: Playback Setup

- Services: `dot-speech-tts` generates `.wav` files; `dot-speech-player` listens for responses; a host watcher auto‑plays files on your OS.

Run the containers
- `docker compose -f cohost-compose.yml up -d rabbitmq dot-speech-tts dot-speech-player`
- Audio files are written to `./shared_audio` on the host.

Host auto‑player (Windows)
- Use the launcher:
  - `./dot_speech_player_host/start-host-watcher.ps1`
  - Optional params: `-RabbitHost localhost -Queue dot.speech.tts.response -RabbitUser root -RabbitPass adminpass`
- Falls back to `.env` for `ADMIN_USER_NAME`/`ADMIN_PASS` if not provided.

Host auto‑player (macOS/Linux)
- Create venv and run:
  - `python3 -m venv .host-venv && . .host-venv/bin/activate && pip install -r dot_speech_player_host/requirements.txt`
  - `export RABBITMQ_HOST=localhost RABBITMQ_USER=root RABBITMQ_PASS=adminpass HOST_AUDIO_DIR="$(pwd)/shared_audio" TTS_OUTPUT_QUEUE=dot.speech.tts.response`
  - `python dot_speech_player_host/player.py`

Send a test message
- From inside the `dot-speech-tts` container:
  - `docker exec -i $(docker ps --filter "name=dot-speech-tts" -q) python -c "import os,json,pika;host=os.environ.get('RABBITMQ_HOST','rabbitmq');user=os.environ.get('RABBITMQ_USER','root');ps=os.environ.get('RABBITMQ_PASS','adminpass');cred=pika.PlainCredentials(user,ps);conn=pika.BlockingConnection(pika.ConnectionParameters(host=host,credentials=cred));ch=conn.channel();ch.queue_declare(queue='dot.speech.tts');msg={'text':'Hello playback!','speaker':'aloy'};ch.basic_publish('', 'dot.speech.tts', json.dumps(msg));print('published')"`

Notes
- On Docker Desktop (Windows/macOS), containers cannot output audio directly. The host watcher is the intended path to hear results.
- On Linux, in‑container playback can be enabled by passing `/dev/snd` and PulseAudio sockets and setting `PLAYER_USE_FFPLAY=1` in `dot-speech-player`.

