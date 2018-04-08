using FeatureToggle.Controllers;
using FeatureToggle.Definitions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FeatureToggle.Web.Tests
{
    public class HomeControllerUnitTests
    {

        HomeController _sut;
        readonly Mock<IFeatureRepository> _repository;

        public HomeControllerUnitTests()
        {
            _repository = new Mock<IFeatureRepository>();
            _sut = new HomeController(_repository.Object);
        }

        [Fact]
        [Trait("Subcutaneous", "")]
        public void FirstTest()
        {
        }
    }
}
