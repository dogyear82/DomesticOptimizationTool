DomesticOptimizationTool

- dot_speech_tts: Python XTTS worker that consumes text, generates audio, and publishes file paths.
- dot_speech_player: Container-side consumer that acknowledges responses and logs paths.
- dot_speech_player_host: Host-side watcher that auto-plays audio using your OS.

Getting Started
- Bring up services:
  - `docker compose -f cohost-compose.yml up -d rabbitmq dot-speech-tts dot-speech-player`
- Run the host watcher to hear audio on your machine:
  - Windows: `./dot_speech_player_host/start-host-watcher.ps1`
  - macOS/Linux: see the doc below.
- Send a test message (example provided in the doc).

Docs
- See `docs/dot_speech_tts_playback.md` for complete steps and options.
