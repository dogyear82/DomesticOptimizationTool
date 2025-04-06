namespace Dot.Tools
{
    public interface IToolFactory
    {
        ITool GetToolByName(string toolName);
    }
    public class ToolFactory : IToolFactory
    {
        private readonly IEnumerable<ITool> _tools;

        public ToolFactory(IEnumerable<ITool> tools)
        {
            _tools = tools;
        }

        public ITool GetToolByName(string toolName)
        {
            var tool = _tools.FirstOrDefault(t => t.GetType().Name == toolName);
            if (tool is null)
                throw new InvalidOperationException($"Tool with name {toolName} not found.");
            return tool;
        }
    }
}
