using Dot.Tools.Attributes;

namespace Dot.Tools
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
