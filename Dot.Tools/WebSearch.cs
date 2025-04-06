
using Dot.Tools.Attributes;

namespace Dot.Tools
{
    [Tool("Search the web for information.")]
    public class WebSearch : ToolBase
    {
        public string Execute(
            [ToolParam("A search query string containing the keywords or phrase the user wants to look up on the internet. This should clearly express what the user is trying to find.")] string searchQuery)
        {
            return $"Convey the following data to the user in a natural way.";
        }
    }
}
