using System;
using Xunit;
using FluentAssertions;
using FeatureToggle.Definitions;

namespace FeatureToggle.DAL.Tests
{
    public class FeatureRepositoryUnitTests
    {
        readonly FeatureRepository _sut; 

        public FeatureRepositoryUnitTests()
        {
            _sut = new FeatureRepository( @"Test.Json" );
        }

        [Fact]
        public void FeatureRepository_ShouldImplementIFeatureRepository()
        {
            _sut.Should().BeAssignableTo<IFeatureRepository>();
        }

        [Fact]
        public void FeatureRepository_SelectNoPattern_ShouldReturnAllFeatures()
        {
            _sut.Select("").Should().HaveCount(4);
        }
    }
}
