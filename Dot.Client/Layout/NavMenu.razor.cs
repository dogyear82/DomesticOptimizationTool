using Dot.API.Gateway;
using Dot.UI.Models;
using Microsoft.AspNetCore.Components;

namespace Dot.Client.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        private IGateway gateway { get; set; }

        private bool collapseNavMenu = true;
        private List<ConversationMenuItem> conversationMenuItems = new();
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                conversationMenuItems = await gateway.NavMenu.GetConversationsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
