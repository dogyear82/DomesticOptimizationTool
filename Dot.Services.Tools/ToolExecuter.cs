using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dot.Services.Tools
{
    public interface IToolExecuter
    {
        Task<string> Execute(string response);
    }

    public class ToolExecuter : IToolExecuter
    {
        private readonly ILogger<ToolExecuter> _logger;
        private readonly IToolFactory _toolFactory;

        public ToolExecuter(ILogger<ToolExecuter> logger, IToolFactory toolFactory)
        {
            _logger = logger;
            _toolFactory = toolFactory;
        }

        public Task<string> Execute(string response)
        {
            _logger.LogInformation($"Deserializing tool response: {response}");
            var toolResponse = JsonConvert.DeserializeObject<ToolResponse>(response);
            _logger.LogInformation($"Executing tool: {toolResponse.ToolName}");
            var tool = _toolFactory.GetToolByName(toolResponse.ToolName);
            return tool.Execute(toolResponse.ToolParams.ToArray());
        }
    }
}
