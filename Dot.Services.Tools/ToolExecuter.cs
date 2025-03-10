using Newtonsoft.Json;

namespace Dot.Services.Tools
{
    public interface IToolExecuter
    {
        Task<string> Execute(string response);
    }

    public class ToolExecuter : IToolExecuter
    {
        private readonly IToolFactory _toolFactory;

        public ToolExecuter(IToolFactory toolFactory)
        {
            _toolFactory = toolFactory;
        }

        public Task<string> Execute(string response)
        {
            var toolResponse = JsonConvert.DeserializeObject<ToolResponse>(response);
            var tool = _toolFactory.GetToolByName(toolResponse.ToolName);
            return tool.Execute(toolResponse.ToolParams.ToArray());
        }
    }
}
