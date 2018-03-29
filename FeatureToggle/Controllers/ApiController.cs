using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureToggle.Controllers
{
    public class ApiController : Controller
    {

        public JsonResult GetFeatures()
        {
            var features = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("hello", "world")
            };

            return Json(features);
        }
    }
}
