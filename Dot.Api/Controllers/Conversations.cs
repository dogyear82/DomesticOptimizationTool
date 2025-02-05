using Dot.DataAccess.Repositories;
using Dot.Models;
using Dot.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dot.API.Controllers
{
    [Route("api/conversations")]
    [ApiController]
    public class Conversations : Controller
    {
        private readonly IRepository _repo;

        public Conversations(IRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConversationVm>> GetConversationById(string id)
        {
            try
            {
                var conversation = await _repo.Conversation.GetConversationById(id);
                var vm = new ConversationVm
                {
                    Id = conversation.Id,
                    Title = conversation.Title,
                    Description = conversation.Summary,
                    Messages = conversation.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.CreatedBy != Role.System)
                    .Select(x => new ConversationMessageVm
                    {
                        Role = x.CreatedBy,
                        Content = x.Content
                    }).ToList()
                };
                return Ok(vm);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
