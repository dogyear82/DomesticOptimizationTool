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
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                synthesizer.SetOutputToDefaultAudioDevice();

                Console.WriteLine(text);
                synthesizer.Speak(text);

                synthesizer.Rate = -2; // Slow down the speech rate (-10 to 10)
                synthesizer.Volume = 80; // Set volume (0 to 100)
            }
        }
    }
}
