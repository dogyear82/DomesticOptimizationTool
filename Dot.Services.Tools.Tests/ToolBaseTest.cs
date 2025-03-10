namespace Dot.Services.Tools.Tests
{
    public class ToolBaseTest
    {
        [Fact]
        public async Task Execute_WhenCalledWithValidParameters_CallsExecuteMethod()
        {
            var tool = new Silly();
            var parameters = new List<ToolParameter> {
                new ToolParameter {
                    Name = "sillyThing1",
                    Value = "foo"
                },
                new ToolParameter {
                    Name = "sillyThing2",
                    Value = "bar"
                }
            };
            var response = await tool.Execute(parameters.ToArray());
            Assert.Equal("Generate something silly with the words foo and bar", response);
        }
    }
}
