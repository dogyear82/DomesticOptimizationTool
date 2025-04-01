using Dot.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Dot.API.Controllers
{
    [Route("api/llms")]
    [ApiController]
    public class Llms : Controller
    {
        private readonly ILogger<Llms> _logger;
        private readonly IRepository _repo;

        public Llms(ILogger<Llms> logger, IRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<Model>> Index()
        {
            try
            {
                return await _repo.Model.GetDownloadedModelsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [HttpGet("names")]
        public async Task<IEnumerable<string>> GetAllModelNames()
        {
            try
            {
                return await _repo.Model.GetDownloadedModelNamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
