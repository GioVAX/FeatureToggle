using System;
using Xunit;
using FluentAssertions;
using FeatureToggle.Definitions;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;

namespace FeatureToggle.DAL.Tests
{
    public class DiskFeatureRepositoryUnitTests
    {
        readonly DiskFeatureRepository _sut;

        public DiskFeatureRepositoryUnitTests()
        {
            _sut = new DiskFeatureRepository("Test.Json");
        }

        [Fact]
        public void FeatureRepository_ShouldImplementIFeatureRepository()
        {
            _sut.Should().BeAssignableTo<IFeatureRepository>();
        }

        [Fact]
        public void FeatureRepository_InitWithNullString_ShouldReturnEmptyListOfFeatures()
        {
            var sut = new DiskFeatureRepository(null);

            sut.Select("").Should().BeEmpty();
        }

        [Fact]
        public void FeatureRepository_InitWithNonExistingFile_ShouldReturnEmptyListOfFeatures()
        {
            var sut = new DiskFeatureRepository("lksflskdjf.oiwefiuwrf");

            sut.Select("").Should().BeEmpty();
        }

        [Fact]
        public void FeatureRepository_Select_ShouldReturnListOfFeatures()
        {
            _sut.Select("").Should()
                .BeOfType<List<FeatureConfiguration>>();
        }

        [Fact]
        public void FeatureRepository_SelectNoPattern_ShouldReturn4ValidFeatures()
        {
            var features = _sut.Select("").ToList();
            using (new AssertionScope())
            {
                features.Should().HaveCount(4);
                features.Should().OnlyContain(fc => !(string.IsNullOrEmpty(fc.Feature) || string.IsNullOrEmpty(fc.Value)));
            }
        }

        [Fact]
        public void FeatureRepository_SelectPatternFeatureToggle_ShouldReturn2FeaturesStartingWithFeatureToggle()
        {
            const string pattern = "FeatureToggle";
            var features = _sut.Select(pattern);

            features.Should().HaveCount(2)
                .And.OnlyContain(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
