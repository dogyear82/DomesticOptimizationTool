using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System.Speech.Synthesis;

namespace Dot.Modules
{
    public interface IVoice 
    {
        void Speak(string text);
        Task Speak2(string text);
    }

    public class Voice : IVoice
    {
        public void Speak(string text)
        {
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                // Output information about all of the installed voices.   
                Console.WriteLine("Installed voices -");
                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    VoiceInfo info = voice.VoiceInfo;
                    if (info.Id == "TTS_MS_EN-US_ZIRA_11.0")
                    {
                        synth.SelectVoice(info.Name);
                    }
                }

                //synth.SetOutputToDefaultAudioDevice();
                Console.WriteLine(text);

                using (var stream = new MemoryStream())
                {
                    synth.SetOutputToWaveStream(stream);
                    synth.Rate = -2; // Slow down the speech rate (-10 to 10)
                    synth.Volume = 100; // Set volume (0 to 100)
                    synth.Speak(text);
                    Play(stream);
                }



            }
        }

        public Task Speak2(string text)
        {
            throw new NotImplementedException();
        }

        private void Play(MemoryStream audioStream)
        {
            audioStream.Position = 0; // Reset the stream position
            using (var reader = new WaveFileReader(audioStream))
            {
                var pitchShifter = new SmbPitchShiftingSampleProvider(reader.ToSampleProvider());
                pitchShifter.PitchFactor = (float)1.1; // E.g., 1.2 for higher pitch, 0.8 for lower pitch

                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(pitchShifter);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
