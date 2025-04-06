namespace Dot.Tools
{
    public class ToolMeta
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ToolParamMeta> Parameters { get; set; }
    }

    public class ToolParamMeta
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ToolResponse
    {
        public string ToolName { get; set; }
        public List<ToolParameter> ToolParams { get; set; }
    }

    public class ToolParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
