using Dot.API.Gateway;
using Dot.Services.Events;
using Microsoft.AspNetCore.Components;

namespace Dot.Client.Components
{
    public partial class ModelSelect : ComponentBase
    {
        [Inject]
        private IGateway gateway { get; set; }
        [Inject]
        private IEventService eventService { get; set; }

        protected string selectedModelName = string.Empty;
        protected List<string> modelNames = new();
        protected string selectedModel = "mistral";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                modelNames = (await gateway.Llms.GetNamesAsync()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnSelectionChanged(string newModel)
        {
            selectedModel = newModel;
            eventService.Emit(Event.ModelSelected, selectedModel);
        }
    }
}
