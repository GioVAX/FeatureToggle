﻿using System.Collections.Generic;
using System.Linq;
using AutoFixture;
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
        readonly Fixture _fixture;
        readonly Mock<IFeatureRepository> _repository;

        public HomeControllerUnitTests()
        {
            _repository = new Mock<IFeatureRepository>();
            _sut = new HomeController(_repository.Object);
            _fixture = new Fixture();
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
                .Returns(new List<FeatureConfiguration>
                {
                    new FeatureConfiguration("hello", "world")
                });

            _sut.Index();

            _repository.Verify(repo => repo.Select(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Index_ShouldReturnModelViewWithEnumerableOfFeatures()
        {
            // Arrange
            var featureList = new List<FeatureConfiguration>
            {
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

        [Theory]
        [InlineData(nameof(HomeController.EditFeature))]
        [InlineData(nameof(HomeController.AddFeature))]
        public void PostFeatures_ShouldBeDecoratedWithHttpPost(string featureName)
        {
            var type = typeof(HomeController);
            var postMethod = type.GetMethod(featureName);

            postMethod.Should().BeDecoratedWith<HttpPostAttribute>();
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
                .BeEquivalentTo("feature", "value");
        }

        [Fact]
        public void EditFeature_ShouldCallRepositoryUpdateFeatureOncePassingTheReceivedValues()
        {
            var feature = _fixture.Create<string>();
            var newValue = _fixture.Create<string>();
            var featureList = new List<FeatureConfiguration>
            {
                new FeatureConfiguration("hello", "world"),
                new FeatureConfiguration("ciao", "mondo"),
            };
            _repository.Setup(mock => mock.Update(feature, newValue))
                .Returns(featureList);

            _sut.EditFeature(feature, newValue);

            _repository.Verify(repo => repo.Update(feature, newValue), Times.Once);
        }

        [Fact]
        public void AddFeature_ShouldAcceptFeatureNameAndFeatureValue()
        {
            var type = typeof(HomeController);
            var addFeatureParameters = type
                .GetMethod(nameof(HomeController.AddFeature))
                .GetParameters()
                .Select(prm => prm.Name);

            addFeatureParameters.Should()
                .BeEquivalentTo("feature", "value");
        }

        [Fact]
        public void AddFeature_ShouldCallRepositoryAddFeatureOncePassingTheReceivedValues()
        {
            var feature = _fixture.Create<string>();
            var newValue = _fixture.Create<string>();
            _repository.Setup(mock => mock.Add(feature, newValue));

            _sut.AddFeature(feature, newValue);

            _repository.Verify(repo => repo.Add(feature, newValue), Times.Once);
        }
    }
}
