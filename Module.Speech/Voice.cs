using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace Dot.Modules.Speech
{
    public interface IVoice 
    {
        void Speak(string text);
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

                synth.SetOutputToDefaultAudioDevice();

                Console.WriteLine(text);
                synth.Speak(text);

                synth.Rate = -2; // Slow down the speech rate (-10 to 10)
                synth.Volume = 80; // Set volume (0 to 100)
            }
        }
    }
}
