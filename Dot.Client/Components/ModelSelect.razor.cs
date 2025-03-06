using Dot.API.Gateway;
using Microsoft.AspNetCore.Components;

namespace Dot.Client.Components
{
    public partial class ModelSelect : ComponentBase
    {
        [Inject]
        private IGateway gateway { get; set; }

        protected string selectedModelName = string.Empty;
        protected List<string> modelNames = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                modelNames = new List<string> { "test1", "test2", "test3" };//await gateway.Llms.GetNamesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
