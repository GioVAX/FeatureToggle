using System.Collections.Generic;
using FeatureToggle.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.Web.Controllers
{
    public class ApiController : Controller
    {
        readonly IFeatureRepository _repository;

        public ApiController( IFeatureRepository repository )
        {
            _repository = repository;
        }

        public JsonResult GetFeatures(string beginningWith)
        {
            var features = _repository.Select(beginningWith);

            return Json(features);
        }

        public JsonResult DeleteFeature(string featurename)
        {
            var features = _repository.DeleteFeature(featurename);
            return Json(features);
        }
    }
}
