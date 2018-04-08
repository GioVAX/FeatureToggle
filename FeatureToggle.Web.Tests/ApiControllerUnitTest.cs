using FeatureToggle.Controllers;
using FeatureToggle.Definitions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace FeatureToggle.Tests
{
    public class ApiControllerUnitTest
    {
        readonly ApiController _sut;
        readonly Mock<IFeatureRepository> _repository;

        public ApiControllerUnitTest()
        {
            _repository = new Mock<IFeatureRepository>();
            _repository.Setup(mock => mock.Select( It.IsAny<string>()))
                .Returns(new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("hello", "world")
                }
            );

            _sut = new ApiController(_repository.Object);
        }

        [Fact]
        [Trait("Subcutaneous", "")]
        public void GetFeature_ReturnsJson()
        {
            var json = _sut.GetFeatures();

            json.Should().BeOfType<JsonResult>();
        }

        [Fact]
        [Trait("Subcutaneous", "")]
        public void GetFeature_ReturnsArrayOfFeatures()
        {
            var json = _sut.GetFeatures();

            json.Value.Should().BeAssignableTo<IEnumerable<KeyValuePair<string, string>>>();
        }

        [Fact]
        [Trait("Subcutaneous", "")]
        public void GetFeature_CallsRepositorySelect()
        {
            var _ = _sut.GetFeatures();

            _repository.Verify(mock => mock.Select(It.IsAny<string>()), Times.Once);
        }
    }
}
