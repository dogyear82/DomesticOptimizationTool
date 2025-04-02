using Dot.Services.Tools.Attributes;

namespace Dot.Services.Tools
{
    [Tool("Used for general conversation when no other tool is appropriate.")]
    public class GeneralChat : ToolBase
    {
        public string Execute()
        {
            return $"Based on the current conversation, continue the conversation in a natural and factual way.";
        }
    }
}
