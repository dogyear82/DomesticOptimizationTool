namespace Dot.Tools.Tests
{
    public class Serious : ITool
    {
        public Task<string> Execute(params ToolParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public ToolMeta GenerateToolMeta()
        {
            throw new NotImplementedException();
        }
    }

    public class ToolFactoryTests
    {
        [Fact]
        public void GetToolByName_WhenCalledWithValidToolName_ReturnsTool()
        {
            var tools = new List<ITool> {
                new Silly(),
                new Serious()
            };
            var toolFactory = new ToolFactory(tools);
            var tool = toolFactory.GetToolByName("Silly");
            Assert.IsType<Silly>(tool);
        }

        [Fact]
        public void GetToolByName_NoMatchingTool_ThrowsException()
        {
            var tools = new List<ITool> {
                new Silly(),
                new Serious()
            };
            var toolFactory = new ToolFactory(tools);
            Assert.Throws<InvalidOperationException>(() => toolFactory.GetToolByName("NotATool"));
        }
    }
}
