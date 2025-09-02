# Use official Python 3.10 base image
FROM python:3.10-slim

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1

# Set working directory
WORKDIR /app

# Install system dependencies required by TTS (ffmpeg, espeak-ng, libsndfile)
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
      ffmpeg \
      libsndfile1 \
      espeak-ng \
      libgl1 \
      libglib2.0-0 \
      git && \
    rm -rf /var/lib/apt/lists/*

# Copy Python requirements first for layer caching (if you have requirements.txt)
COPY requirements.txt .

# Install dependencies
RUN pip install --upgrade pip && \
    pip install -r requirements.txt

# Copy application code
COPY . .

# Copy model and voices (avoid copying local virtualenvs)
COPY models/xtts/ models/xtts/
COPY voices/ voices/

# Set entrypoint or command (edit as needed)
CMD ["python", "dot_speech_tts.py"]
