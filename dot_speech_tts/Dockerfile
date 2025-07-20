# Use official Python 3.10 base image
FROM python:3.10-slim

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1

# Set working directory
WORKDIR /app

# Copy Python requirements first for layer caching (if you have requirements.txt)
COPY requirements.txt .

# Install dependencies
RUN pip install --upgrade pip && \
    pip install -r requirements.txt

# Copy application code
COPY . .

# Copy specific folders
COPY models/xtts/ models/xtts/
COPY tts/ tts/
COPY voices/ voices/

# Set entrypoint or command (edit as needed)
CMD ["python", "dot_speech_tts.py"]
