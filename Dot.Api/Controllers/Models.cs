using Dot.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using OllamaSharp.Models;

namespace Dot.API.Controllers
{
    [Route("api/models")]
    [ApiController]
    public class Models : Controller
    {
        private readonly ILogger<Models> _logger;
        private readonly IRepository _repo;

        public Models(ILogger<Models> logger, IRepository repo)
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
