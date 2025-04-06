
namespace Dot.Tools.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolAttribute : Attribute
    {
        public string Description { get; }

        public ToolAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ToolParamAttribute : Attribute
    {
        public string Description { get; }
        public bool IsRequired { get; set; }

        public ToolParamAttribute(string description, bool isRequired = true)
        {
            Description = description;
            IsRequired = isRequired;
        }
    }
}
