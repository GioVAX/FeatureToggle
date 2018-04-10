using System;
using FeatureToggle.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.Web.Controllers
{
    public class ApiController : Controller
    {
        readonly IFeatureRepository _repository;

        public ApiController(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public JsonResult GetFeatures(string beginningWith)
        {
            var features = _repository.Select(beginningWith);

            return Json(features);
        }

        public IActionResult DeleteFeature(string featureName)
        {
            _repository.Delete(featureName);

            return new JsonResult("") { StatusCode = 203 };
        }
    }
}
