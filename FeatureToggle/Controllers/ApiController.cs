using FeatureToggle.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.Web.Controllers
{
    [Produces("application/json")]
    public class ApiController : Controller
    {
        readonly IFeatureRepository _repository;

        public ApiController(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public IActionResult GetFeatures(string beginningWith)
        {
            var features = _repository.Select(beginningWith);

            return Ok(features);
        }
    }
}
