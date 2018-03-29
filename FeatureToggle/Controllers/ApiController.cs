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
            return Json("hello");
        }
    }
}
