using Microsoft.Extensions.AI;

namespace Dot.Models
{
    public class ChatStream
    {

        public string Role { get; set; }
        public string Text { get; set; }
        public bool IsDone { get; set; }

        // Parameterless constructor is required for JSON deserialization
        public ChatStream() { }

        public ChatStream(ChatResponseUpdate stream)
        {
            Role = (stream.Role.HasValue ? stream.Role.Value : ChatRole.Assistant).ToString();
            Text = stream.Text;
            IsDone = stream.FinishReason.HasValue;
        }
    }
}
