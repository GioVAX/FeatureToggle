using System.Diagnostics;
using FeatureToggle.Definitions;
using FeatureToggle.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFeatureRepository _repository;

        public HomeController(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index()
        {
            var features = _repository.Select("");

            return View(features);
        }

        public IActionResult DeleteFeature(string featureName)
        {
            _repository.Delete(featureName);

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult EditFeature(string feature, string value)
        {
            _repository.Update(feature, value);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult AddFeature(string feature, string value)
        {
            _repository.Add(feature, value);
            return RedirectToAction(nameof(Index));
        }
    }
}
