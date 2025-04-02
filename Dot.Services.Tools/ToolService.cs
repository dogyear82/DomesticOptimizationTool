
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Dot.Services.Tools
{
    public interface IToolService
    {
        string ConstructToolPrompt();
    }

    public class ToolService : IToolService
    {
        private readonly IToolExecuter _toolExecuter;
        private readonly IToolFactory _toolFactory;
        private readonly IEnumerable<ITool> _tools;
        private readonly string _toolPrompt;

        public ToolService(IEnumerable<ITool> tools)
        {
            _tools = tools;
            //_toolFactory = sp.GetRequiredService<IToolFactory>();
            //_toolExecuter = sp.GetRequiredService<IToolExecuter>();
            _toolPrompt = ConstructToolPrompt();
        }

        public string ConstructToolPrompt()
        {
            var toolMeta = new List<ToolMeta>();
            foreach (var tool in _tools)
            {
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
    }
}
