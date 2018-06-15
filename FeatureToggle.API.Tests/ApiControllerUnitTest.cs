using System;
using System.Collections.Generic;
using System.Reflection;
using FeatureToggle.Definitions;
using FeatureToggle.API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FeatureToggle.API.Tests
{
    [Trait("Subcutaneous", "")]
    public class ApiControllerUnitTest
    {
        readonly ApiController _sut;
        readonly Mock<IFeatureRepository> _repository;

        public ApiControllerUnitTest()
        {
            _repository = new Mock<IFeatureRepository>();

            _sut = new ApiController(_repository.Object);
        }

        [Fact]
        public void ApiController_ShouldOnlyHaveGetFeaturesEndpoint()
        {
            var type = typeof(ApiController);

            type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Should().HaveCount(1, "because the API should only allow retrieving features.")
                .And.Contain(method => string.Equals(method.Name, "GetFeatures", StringComparison.InvariantCultureIgnoreCase), "because the API should only allow retrieving features.");
        }

        [Fact]
        public void GetFeature_ReturnsObjectResult()
        {
            var result = _sut.GetFeatures("");

            result.Should().BeAssignableTo<ObjectResult>();
        }

        [Fact]
        public void GetFeature_ReturnsArrayOfFeatures()
        {
            _repository.Setup(mock => mock.Select(It.IsAny<string>()))
                .Returns(new List<FeatureConfiguration> {
                    new FeatureConfiguration("hello", "world")
                });

            var json = (_sut.GetFeatures("") as ObjectResult)?.Value;

            json
                .Should().NotBeNull()
                .And.BeAssignableTo<IEnumerable<FeatureConfiguration>>();
        }

        [Fact]
        public void GetFeature_CallsRepositorySelect()
        {
            var _ = _sut.GetFeatures("");

            _repository.Verify(mock => mock.Select(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetFeature_WithParameter_ReturnsFilteredList()
        {
            const string pattern = "hello";
            _repository.Setup(mock => mock.Select(It.IsAny<string>()))
                .Returns(new List<FeatureConfiguration> {
                    new FeatureConfiguration("hello", "world")
                });

            var result = _sut.GetFeatures(pattern);

            var features = (result as ObjectResult)?.Value as IEnumerable<FeatureConfiguration>;

            features
                .Should().NotBeNull()
                .And.HaveCount(1)
                .And.OnlyContain(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
