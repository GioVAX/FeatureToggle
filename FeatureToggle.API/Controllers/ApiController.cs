using FeatureToggle.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.API.Controllers
{
    [Produces("application/json")]
    [Route( "api/Feature/[action]")]
    public class ApiController : Controller
    {
        readonly IFeatureRepository _repository;

        public ApiController(IFeatureRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetFeatures(string beginningWith)
        {
            var features = _repository.Select(beginningWith);

            return Ok(features);
        }
    }
}
