using Dot.DataAccess.Repositories;
using Dot.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dot.Api.Controllers
{
    [Route("api/navmenu")]
    [ApiController]
    public class NavMenu : Controller
    {
        private readonly IRepository _repo;

        public NavMenu(IRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("conversations")]
        public async Task<ActionResult<List<ConversationMenuItem>>> GetConversationMenuItems()
        {
            try
            {
                var allConversations = await _repo.Conversation.GetAllConversationsAsync();
                var conversations = allConversations is null || !allConversations.Any() ? 
                    new List<ConversationMenuItem>() :
                    allConversations.Select(x => new ConversationMenuItem
                    {
                        ConversationId = x.Id,
                        Text = x.Title
                    });
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
