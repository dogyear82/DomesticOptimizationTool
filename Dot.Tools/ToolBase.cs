using Dot.Tools.Attributes;
using System.Reflection;

namespace Dot.Tools
{
    public class ToolBase : ITool
    {
        public async Task<string> Execute(params ToolParameter[] parameters)
        {
            return InvokeExecuteWithReflection(parameters);
        }

        protected string InvokeExecuteWithReflection(ToolParameter[] parameters)
        {
            var method = GetType().GetMethods()
                .FirstOrDefault(m => m.Name == "Execute" && !m.GetParameters().Any(p => p.ParameterType == typeof(object[])));

            if (method == null)
                throw new InvalidOperationException("No matching Execute method found for the provided parameters.");

            // Get method parameters
            var methodParams = method.GetParameters();
            if (methodParams.Length == 0)
            {
                return (string)method.Invoke(this, null);
            }

            // Convert values to correct types
            var orderedParams = methodParams.Select(paramInfo =>
            {
                var matchingToolParam = parameters.FirstOrDefault(tp => tp.Name == paramInfo.Name);
                if (matchingToolParam == null)
                    throw new ArgumentException($"Missing required parameter: {paramInfo.Name}");

                return Convert.ChangeType(matchingToolParam.Value, paramInfo.ParameterType);
            }).ToArray();


            return (string)method.Invoke(this, orderedParams);
        }

        public ToolMeta GenerateToolMeta()
        {
            var toolType = GetType();
            var executeMethod = toolType.GetMethods()
                .FirstOrDefault(m => m.Name == "Execute" && !m.GetParameters().Any(p => p.ParameterType == typeof(object[])));

            if (executeMethod == null)
                throw new InvalidOperationException("Tool must implement an Execute method with explicit parameters.");

            return new ToolMeta
            {
                Name = toolType.Name,
                Description = toolType.GetCustomAttribute<ToolAttribute>().Description,
                Parameters = GetParameterMetadata(executeMethod)
            };
        }

        private List<ToolParamMeta> GetParameterMetadata(MethodInfo method)
        {
            var paramsMeta = new List<ToolParamMeta>();

            var parameters = method.GetParameters();
            if (parameters is null || parameters.Length < 1)
                return paramsMeta;

            foreach (var param in parameters)
            {
                if (string.IsNullOrWhiteSpace(param.Name))
                {
                    throw new InvalidOperationException("Tool parameter must have a name.");
                }

                var paramAttr = param.GetCustomAttribute<ToolParamAttribute>();
                paramsMeta.Add(new ToolParamMeta
                {
                    Name = param.Name,
                    Description = paramAttr?.Description ?? param.Name,
                    Type = param.ParameterType.Name,
                    IsRequired = paramAttr?.IsRequired ?? false
                });
            }
            return paramsMeta;
        }
    }
}
