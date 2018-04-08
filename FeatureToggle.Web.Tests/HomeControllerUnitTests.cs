using FeatureToggle.Controllers;
using FeatureToggle.Definitions;
using FluentAssertions;
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
        [Trait("Subcutaneous","")]
        public void Index_ShouldCallRepositoryGetFeatures()
        {
            _repository.Setup(mock => mock.Select(It.IsAny<string>()))
                .Returns(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("hello", "world")
                });

            _sut.Index();

            _repository.Verify(repo => repo.Select( It.IsAny<string>() ), Times.Once);
        }
    }
}
