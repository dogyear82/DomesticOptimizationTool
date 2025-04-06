using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dot.Tools
{
    public interface IToolService
    {
        string GetToolPrompt();
        Task<string> Process(string toolResponse);
    }

    public class ToolService : IToolService
    {
        private readonly ILogger<ToolService> _logger;
        private readonly IToolExecuter _toolExecuter;
        private readonly IEnumerable<ITool> _tools;
        private readonly string _toolPrompt;

        public ToolService(ILogger<ToolService> logger, IServiceProvider sp, IEnumerable<ITool> tools)
        {
            _logger = logger;
            _tools = tools;
            _toolExecuter = sp.GetRequiredService<IToolExecuter>();
            _toolPrompt = ConstructToolPrompt();
        }

        private string ConstructToolPrompt()
        {
            try
            {
                var toolMeta = new List<ToolMeta>();
                foreach (var tool in _tools)
                {
                    _logger.LogInformation("Generating tool meta for {tool}", tool.GetType().Name);
                    var meta = tool.GenerateToolMeta();
                    if (meta != null)
                    {
                        toolMeta.Add(meta);
                    }
                }
                var toolDefinitions = JsonConvert.SerializeObject(toolMeta);

                var sampleResponse = new ToolResponse
                {
                    ToolName = "Name of the Tool",
                    ToolParams = new List<ToolParameter>
                {
                    new ToolParameter
                    {
                        Name = "Name of the Tool parameter",
                        Value = "Value of the Tool parameter"
                    }
                }
                };
                var sampleSchema = JsonConvert.SerializeObject(sampleResponse);

                return $"You are a tool-using assistant. Based on the following conversation, choose the most appropriate tool to execute along with the required parameters. Respond ONLY as JSON using this format: {sampleSchema}. Here are the available tools: {toolDefinitions}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing tool prompt");
                return "Error constructing tool prompt";
            }
        }

        public string GetToolPrompt()
        {
            if (string.IsNullOrWhiteSpace(_toolPrompt))
            {
                ConstructToolPrompt();
            }

            return _toolPrompt;
        }

        public async Task<string> Process(string toolResponse)
        {
            _logger.LogInformation("Prcessing tool response");
            return await _toolExecuter.Execute(toolResponse);
        }
    }
}
