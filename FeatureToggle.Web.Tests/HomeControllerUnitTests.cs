using System.Collections.Generic;
using System.Linq;
using FeatureToggle.Definitions;
using FeatureToggle.Web.Controllers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FeatureToggle.Web.Tests
{
    [Trait("Subcutaneous", "")]
    public class HomeControllerUnitTests
    {
        readonly HomeController _sut;
        readonly Mock<IFeatureRepository> _repository;

        public HomeControllerUnitTests()
        {
            _repository = new Mock<IFeatureRepository>();
            _sut = new HomeController(_repository.Object);
        }

        [Fact]
        public void Index_ReturnsView()
        {
            var view = _sut.Index();
            view.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Index_ShouldCallRepositoryGetFeatures()
        {
            _repository.Setup(mock => mock.Select(It.IsAny<string>()))
                .Returns(new List<FeatureConfiguration> {
                    new FeatureConfiguration("hello", "world")
                });

            _sut.Index();

            _repository.Verify(repo => repo.Select(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Index_ShouldReturnModelViewWithEnumerableOfFeatures()
        {
            // Arrange
            var featureList = new List<FeatureConfiguration> {
                                new FeatureConfiguration("hello", "world"),
                                new FeatureConfiguration("ciao", "mondo"),
                };

            _repository.Setup(mock => mock.Select(It.IsAny<string>()))
                .Returns(featureList);

            //Act
            var viewModel = _sut.Index().Model;

            //Assert
            using (new AssertionScope())
            {
                viewModel.Should().BeAssignableTo<IEnumerable<FeatureConfiguration>>();

                var modelList = (IEnumerable<FeatureConfiguration>)viewModel;

                modelList.Should().HaveCount(2)
                    .And.BeEquivalentTo(featureList);
            }
        }

        [Fact]
        public void EditFeature_ShouldBeDecoratedWithPost()
        {
            var type = typeof(HomeController);
            var editFeatureMethod = type.GetMethod(nameof(HomeController.EditFeature));

            editFeatureMethod.Should().BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void EditFeature_ShouldAcceptFeatureNameAndFeatureValue()
        {
            var type = typeof(HomeController);
            var editFeatureParameters = type
                .GetMethod(nameof(HomeController.EditFeature))
                .GetParameters()
                .Select(prm => prm.Name);

            editFeatureParameters.Should()
                .BeEquivalentTo(new[] { "feature", "value" });
        }
    }
}
