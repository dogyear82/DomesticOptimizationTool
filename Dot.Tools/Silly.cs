
using Dot.Tools.Attributes;

namespace Dot.Tools
{
    [Tool("Generates a silly string based on 2 input parameters.")]
    public class Silly : ToolBase
    {
        public string Execute(
            [ToolParam("First word to use for generating a silly string.")] string sillyThing1,
            [ToolParam("Second word to use for generating a silly string.", false)] string sillyThing2)
        {
            return $"Generate something silly with the words {sillyThing1} and {sillyThing2}";
        }
    }
}
