using FeatureToggle.Controllers;
using FeatureToggle.Definitions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
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
        public void Index_ReturnsView()
        {
            var view = _sut.Index();
            view.Should().BeOfType<ViewResult>();
        }

        [Fact]
        [Trait("Subcutaneous", "")]
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
        [Trait("Subcutaneous", "")]
        public void Index_ShouldReturnModelViewWithEnumerableOfFeatures()
        {
            var featureList = new List<FeatureConfiguration> {
                                new FeatureConfiguration("hello", "world"),
                                new FeatureConfiguration("ciao", "mondo"),
                };

            _repository.Setup(mock => mock.Select(It.IsAny<string>()))
                .Returns(featureList);

            var viewModel = _sut.Index().Model;

            using (new AssertionScope())
            {
                viewModel.Should().BeAssignableTo<IEnumerable<FeatureConfiguration>>();

                var modelList = (IEnumerable<FeatureConfiguration>)viewModel;

                modelList.Should().HaveCount(2)
                    .And.BeEquivalentTo(featureList);
            }
        }
    }
}
