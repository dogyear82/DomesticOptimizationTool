using System;
using System.Threading.Tasks;
using AI.Gateway;
using AI.Gateway.API.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dot
{
    public class Function2
    {
        private readonly ILogger _logger;
        private readonly IAiGateway _gateway;

        public Function2(ILogger<Function2> logger, IServiceProvider serviceProvider) 
        {
            _logger = logger;
            _gateway = serviceProvider.GetRequiredService<IAiGateway>();
        }

        [Function("Function2")]
        public async Task RunAsync([TimerTrigger("* * * * * *")] TimerInfo myTimer)
        {
            var request = new TtsRequest()
            {
                Type = "Text_To_SPEECH",
                Model = "google-tts",
                Prompt = new TtsPrompt
                {
                    AudioEncoding = "MP3",
                    LanguageCode = "en-US",
                    Name = "en-US-Casual-K",
                    Pitch = 0,
                    SpeakingRate = 1,
                    Text = "hi",
                    VolumeGainDb = 0
                }
            };
            var ttsResult = await _gateway.TextToSpeech.GetAsync(request);
            //var ddd = new Voice();
            //ddd.Speak("Hello Tan.  How can I help you today?");
            //
            //// Initialize a new instance of the SpeechSynthesizer.  
            //using (SpeechSynthesizer synth = new SpeechSynthesizer())
            //{
            //
            //    // Output information about all of the installed voices.   
            //    Console.WriteLine("Installed voices -");
            //    foreach (InstalledVoice voice in synth.GetInstalledVoices())
            //    {
            //        VoiceInfo info = voice.VoiceInfo;
            //        string AudioFormats = "";
            //        foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats)
            //        {
            //            AudioFormats += string.Format("{0}\n",
            //            fmt.EncodingFormat.ToString());
            //        }
            //
            //        Console.WriteLine(" Name:          " + info.Name);
            //        Console.WriteLine(" Culture:       " + info.Culture);
            //        Console.WriteLine(" Age:           " + info.Age);
            //        Console.WriteLine(" Gender:        " + info.Gender);
            //        Console.WriteLine(" Description:   " + info.Description);
            //        Console.WriteLine(" ID:            " + info.Id);
            //        Console.WriteLine(" Enabled:       " + voice.Enabled);
            //        if (info.SupportedAudioFormats.Count != 0)
            //        {
            //            Console.WriteLine(" Audio formats: " + AudioFormats);
            //        }
            //        else
            //        {
            //            Console.WriteLine(" No supported audio formats found");
            //        }
            //
            //        string AdditionalInfo = "";
            //        foreach (string key in info.AdditionalInfo.Keys)
            //        {
            //            AdditionalInfo += string.Format("  {0}: {1}\n", key, //info.AdditionalInfo[key]);
            //        }
            //
            //        Console.WriteLine(" Additional Info - " + AdditionalInfo);
            //        Console.WriteLine();
            //    }
            //}
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();

            //var factory = new ConnectionFactory();
            //factory.Uri = new Uri(Environment.GetEnvironmentVariable("MessageBusConnection"));
            //
            //using (var connection = await factory.CreateConnectionAsync())
            //using (var channel = await connection.CreateChannelAsync())
            //{
            //    var exchangeName = "my-test-exchange"; // Topic exchange name
            //    var routingKey = "testkey"; // Routing key
            //    var queueName = "testqueue";
            //
            //    var message = "An error occurred.";
            //    var body = Encoding.UTF8.GetBytes(message);
            //
            //    await channel.ExchangeDeclareAsync(exchange: exchangeName, durable: true, type: ExchangeType.Topic);
            //    await channel.QueueDeclareAsync(queueName, true, false, false, null);
            //    await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);
            //
            //
            //
            //    byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello, world!");
            //    var props = new BasicProperties();
            //    props.ContentType = "text/plain";
            //    props.DeliveryMode = DeliveryModes.Persistent;
            //    await channel.BasicPublishAsync(exchangeName, routingKey, false, props, messageBodyBytes);
            //
            //    Console.WriteLine($"[x] Sent '{message}' with routing key '{routingKey}'");
            //}
        }
    }
}
