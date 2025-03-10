namespace Dot.Services.Tools.Tests
{
    public class ToolBaseTests
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

        [Fact]
        public void GenerateToolMeta_ForEachClassThatInheritsFromITool_ReturnsExpectedMetaData()
        {
            var meta = new Silly().GenerateToolMeta();

            Assert.Equal("Silly", meta.Name);
            Assert.Equal("Generates a silly string based on 2 input parameters.", meta.Description);
            Assert.Equal(2, meta.Parameters.Count);

            var firstParam = meta.Parameters.FirstOrDefault(meta => meta.Name == "sillyThing1");
            Assert.Equal("sillyThing1", firstParam.Name);
            Assert.Equal("First word to use for generating a silly string.", firstParam.Description);
            Assert.True(firstParam.IsRequired);

            var secondParam = meta.Parameters.FirstOrDefault(meta => meta.Name == "sillyThing2");
            Assert.Equal("sillyThing2", secondParam.Name);
            Assert.Equal("Second word to use for generating a silly string.", secondParam.Description);
            Assert.False(secondParam.IsRequired);
        }
    }
}
