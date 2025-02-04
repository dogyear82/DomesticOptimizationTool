using Dot.Client.Services;
using Dot.UI.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Dot.Client.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        private IHttpClientFactory httpClientFactory { get; set; }

        private bool collapseNavMenu = true;
        private List<ConversationMenuItem> conversationMenuItems = new();
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var client = httpClientFactory.GetHttpclient();
                var response = await client.GetAsync("api/sidebar/conversations");
                Console.WriteLine(response.ReasonPhrase);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (content is not null)
                    {
                        conversationMenuItems = JsonConvert.DeserializeObject<List<ConversationMenuItem>>(content);
                    }
                }
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
