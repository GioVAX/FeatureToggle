using FeatureToggle.Definitions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureToggle.Controllers
{
    public class ApiController : Controller
    {
        IFeatureRepository _repository;

        public ApiController( IFeatureRepository repository )
        {
            _repository = repository;
        }

        public JsonResult GetFeatures(string beginningWith)
        {
            var features = _repository.Select(beginningWith);

            return Json(features);
        }
    }
}
