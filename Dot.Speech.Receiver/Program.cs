// This sample assumes you're using .NET on Windows
// Required NuGet packages:
// - Microsoft.AspNetCore.SignalR.Client
// - System.Speech (for TTS)
// - NAudio (for audio capture)
// - whisper-cpp-dotnet (your Whisper bindings)

using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using Whisper.net;
using VadSharp;
using Dot.Models;
using System.Diagnostics;
using System.Media;

namespace WhisperSignalRApp
{
    class Program
    {
        private const int SAMPLE_RATE = 16000;
        private const float THRESHOLD = 0.5f;
        private const int MIN_SPEECH_DURATION_MS = 250;
        private const float MAX_SPEECH_DURATION_SECONDS = float.PositiveInfinity;
        private const int MIN_SILENCE_DURATION_MS = 100;
        private const int SPEECH_PAD_MS = 30;

        private static string modelPath = Path.Combine(AppContext.BaseDirectory, "resources", "silero_vad.onnx");
        private static string whisperModelPath = "ggml-base.en.bin";
        private static string outputFilePath = "recorded.wav";
        private static bool inSpeech = false;
        private static List<byte> currentSpeechBuffer = new();
        private static VadDetector vadDetector;
        private static WaveInEvent waveIn;
        private static DateTime lastSpeechTime;
        private static TimeSpan silenceTimeout = TimeSpan.FromMilliseconds(4000);
        private static object bufferLock = new(); 
        private static string conversationId = "67fddd6bbf4d1aff218e96e8";
        private static string model = "openhermes";

        static async Task Main(string[] args)
        {
            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"Model file not found: {modelPath}");
                return;
            }

            vadDetector = new VadDetector(modelPath, THRESHOLD, SAMPLE_RATE, MIN_SPEECH_DURATION_MS, MAX_SPEECH_DURATION_SECONDS, MIN_SILENCE_DURATION_MS, SPEECH_PAD_MS);

            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(SAMPLE_RATE, 1),
                DeviceNumber = 0,
                BufferMilliseconds = 100
            };

            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();

            Console.WriteLine("Listening started. Speak to interact...");
            bool wasInSpeech = false;
            while (true)
            {
                bool shouldProcess = false;

                lock (bufferLock)
                {
                    bool hasTimedOut = DateTime.UtcNow - lastSpeechTime > silenceTimeout;
                    bool hasBufferedAudio = currentSpeechBuffer.Count > 0;
                    bool hasPreviousSpeech = lastSpeechTime > DateTime.MinValue;
                    if (wasInSpeech && hasTimedOut && hasBufferedAudio && hasPreviousSpeech)
                    {
                        waveIn.StopRecording();
                        Console.WriteLine("Silence detected after speech. Processing...");
                        using (var ms = new MemoryStream(currentSpeechBuffer.ToArray()))
                        using (var rawSource = new RawSourceWaveStream(ms, new WaveFormat(16000, 16, 1)))
                        using (var writer = new WaveFileWriter(outputFilePath, rawSource.WaveFormat))
                        {
                            rawSource.CopyTo(writer);
                        }

                        currentSpeechBuffer.Clear();
                        shouldProcess = true;
                    }

                    wasInSpeech = inSpeech;
                }

                if (shouldProcess)
                {
                    await ProcessAndRespond();

                    lock (bufferLock)
                    {
                        inSpeech = false;
                        wasInSpeech = false;
                        lastSpeechTime = DateTime.MinValue;
                        currentSpeechBuffer.Clear();
                        waveIn.StartRecording();
                        Console.WriteLine("DOT response complete. Listening resumed. Speak to interact...");
                    }
                }

                await Task.Delay(1000);
            }
        }

        static void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            float[] floatSamples = new float[e.BytesRecorded / 2];
            for (int i = 0; i < floatSamples.Length; i++)
            {
                short sample = BitConverter.ToInt16(e.Buffer, i * 2);
                floatSamples[i] = sample / 32768f;
            }

            lock (bufferLock)
            {
                currentSpeechBuffer.AddRange(e.Buffer[..e.BytesRecorded]);

                short[] samples = new short[currentSpeechBuffer.Count / 2];
                Buffer.BlockCopy(currentSpeechBuffer.ToArray(), 0, samples, 0, currentSpeechBuffer.Count);
                float[] fullFloatSamples = samples.Select(s => s / 32768f).ToArray();

                var segments = vadDetector.GetSpeechSegmentList(fullFloatSamples, 16000); 
                var validSegments = segments
                    .Where(seg => (seg.EndSecond - seg.StartSecond) > 0.5f)
                    .ToList();
                if (validSegments.Count > 0 && !inSpeech)
                {
                    inSpeech = true;
                    lastSpeechTime = DateTime.UtcNow;
                }
            }
        }

        static async Task ProcessAndRespond()
        {
            var processor = WhisperFactory.FromPath(whisperModelPath).CreateBuilder().Build();
            using var fileStream = File.OpenRead(outputFilePath);
            var transcribedText = "";
            await foreach (var result in processor.ProcessAsync(fileStream))
            {
                transcribedText += result.Text;
            }

            Console.WriteLine($"You said: {transcribedText}");

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:8080/cohosthub")
                .WithAutomaticReconnect()
                .Build();

            await connection.StartAsync();

            var responseBuffer = new StringBuilder();
            var tcs = new TaskCompletionSource();

            connection.On<ChatStream>("ReceiveMessage", chunk =>
            {
                Console.WriteLine($"[SignalR] Received Chunk: {chunk.Text} | IsDone: {chunk.IsDone}");
                responseBuffer.Append(chunk.Text);
                if (chunk.IsDone)
                    tcs.TrySetResult();
            });

            await connection.SendAsync("SendMessage", transcribedText, model, conversationId);
            await tcs.Task;

            var finalText = responseBuffer.ToString();
            if (!string.IsNullOrWhiteSpace(finalText))
            {
                Speak(finalText, "tifa");
            }
        }

        private static void Speak(string text, string speaker)
        {
            string pythonExe = @"C:\Users\thngu\source\repos\DomesticOptimizationTool\Dot.Speech.Receiver\tts\Scripts\python.exe";
            string scriptPath = "coqui_tts.py";
            string args = $"\"{text}\" \"{speaker}\"";

            var psi = new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = $"{scriptPath} {args}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }; 
            
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;

            var process = Process.Start(psi);

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine("Python stdout:\n" + stdout);
            Console.WriteLine("Python stderr:\n" + stderr);

            string outputPath = "ai-response.wav";

            // Play audio once it's generated
            using (SoundPlayer player = new SoundPlayer(outputPath))
            {
                player.Play();
            }
        }
    }

    static class AudioRecorder
    {
        public static async Task RecordToFileAsync(string path)
        {
            // You can wire up NAudio to capture mic input and save to WAV here
            // Placeholder:
            Console.WriteLine("[Placeholder] Simulated recording to: " + path);
            File.WriteAllBytes(path, new byte[0]); // simulate empty file
            await Task.CompletedTask;
        }
    }

    static class WhisperTranscriber
    {
        public static string Transcribe(string filePath)
        {
            // Call into whisper.cpp or your Whisper binding here
            // Placeholder return
            return "Hello, how are you?";
        }
    }
}
