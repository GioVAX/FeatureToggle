using System;
using Xunit;
using FluentAssertions;
using FeatureToggle.Definitions;
using System.Collections.Generic;

namespace FeatureToggle.DAL.Tests
{
    public class FeatureRepositoryUnitTests
    {
        readonly DiskFeatureRepository _sut;

        public FeatureRepositoryUnitTests()
        {
            _sut = new DiskFeatureRepository(@"Test.Json");
        }

        [Fact]
        public void FeatureRepository_ShouldImplementIFeatureRepository()
        {
            _sut.Should().BeAssignableTo<IFeatureRepository>();
        }

        [Fact]
        public void FeatureRepository_Select_ShouldReturnListOfFeatures()
        {
            _sut.Select("").Should()
                .BeOfType<List<KeyValuePair<string, string>>>();
        }

        [Fact]
        public void FeatureRepository_SelectNoPattern_ShouldReturn4Features()
        {
            _sut.Select("").Should().HaveCount(4);
        }

        [Fact]
        public void FeatureRepository_SelectPatternFeatureToggle_ShouldReturn2FeaturesStartingWithFeatureToggle()
        {
            var pattern = "FeatureToggle";
            _sut.Select(pattern).Should().HaveCount(2)
                .And.OnlyContain(pair => pair.Key.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
