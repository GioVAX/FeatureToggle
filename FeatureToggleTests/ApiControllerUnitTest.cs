using FeatureToggle.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace FeatureToggleTests
{
    public class ApiControllerUnitTest
    {
        readonly ApiController _sut;

        public ApiControllerUnitTest()
        {
            _sut = new ApiController();
        }

        [Fact]
        public void GetFeature_ReturnsJson()
        {
            var json = _sut.GetFeatures();

            Assert.IsType<JsonResult>(json);
        }

        [Fact]
        public void GetFeature_ReturnsArrayOfFeatures()
        {
            var json = _sut.GetFeatures();

            Assert.IsType<KeyValuePair<string, string>[]>(json.Value);
        }
    }
}
