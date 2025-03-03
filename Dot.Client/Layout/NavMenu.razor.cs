using Dot.API.Gateway;
using Dot.Services.Events;
using Dot.UI.Models;
using Microsoft.AspNetCore.Components;

namespace Dot.Client.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        private IGateway gateway { get; set; }
        [Inject]
        private IEventService eventService { get; set; }

        private bool collapseNavMenu = true;
        private List<ConversationMenuItem> conversationMenuItems = new();
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            await GetMenuItemsAsync();
            eventService.Subscribe(Event.NewConversationCreated, async (obj) => await GetMenuItemsAsync());

        }

        private async Task GetMenuItemsAsync()
        {
            try
            {
                conversationMenuItems = await gateway.NavMenu.GetConversationsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
