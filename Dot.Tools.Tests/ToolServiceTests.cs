using Moq;

namespace Dot.Tools.Tests
{
    public class ToolServiceTests
    {
        private readonly IToolService _toolService;

        public ToolServiceTests()
        {
            var tools = new List<ITool>
            {
                new Silly(),
                new GeneralChat()
            };
            _toolService = new ToolService(new Mock<IServiceProvider>().Object, tools);
        }

        [Fact]
        public void ConstructToolPrompt_ReturnsPrompt()
        {
            var prompt = _toolService.GetToolPrompt();
            Assert.NotNull(prompt);
        }
    }
}
