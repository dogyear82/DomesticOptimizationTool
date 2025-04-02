using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Services.Tools.Tests
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
            _toolService = new ToolService(tools);
        }

        [Fact]
        public void ConstructToolPrompt_ReturnsPrompt()
        {
            var prompt = _toolService.ConstructToolPrompt();
            Assert.NotNull(prompt);
        }
    }
}
