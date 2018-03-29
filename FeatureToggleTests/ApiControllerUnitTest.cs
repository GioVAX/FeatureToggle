using FeatureToggle.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace FeatureToggleTests
{
    public class ApiControllerUnitTest
    {
        ApiController _sut;

        public ApiControllerUnitTest()
        {
            _sut = new ApiController();
        }

        [Fact]
        public void GetFeature_ReturnsJson()
        {
            var features = _sut.GetFeatures();

            Assert.IsType<JsonResult>(features);
        }
    }
}
