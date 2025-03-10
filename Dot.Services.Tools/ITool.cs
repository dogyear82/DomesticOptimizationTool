namespace Dot.Services.Tools
{
    public interface ITool
    {
        Task<string> Execute(params ToolParameter[] parameters);
        ToolMeta GenerateToolMeta();
    }
}
