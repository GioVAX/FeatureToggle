using System;
using Xunit;
using FluentAssertions;
using FeatureToggle.Definitions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions.Execution;

namespace FeatureToggle.DAL.Tests
{
    public class DiskFeatureRepositoryUnitTests : IDisposable
    {
        private readonly DiskFeatureRepository _sut;
        private readonly string _destFileName = "Test_Tmp.Json";
        private readonly Fixture _fixture = new Fixture();

        public DiskFeatureRepositoryUnitTests()
        {
            File.Copy("Test.Json", _destFileName);

            _sut = new DiskFeatureRepository(_destFileName);
        }

        public void Dispose()
        {
            File.Delete(_destFileName);
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

        [Fact]
        public void FeatureRepository_DeleteExisitingFeature_ShouldRemoveTheFeature()
        {
            const string featureName = "OtherRoot.Font";

            _sut.Delete(featureName);

            _sut.Select("")
                .Should().HaveCount(3)
                .And.NotContain(configuration => string.Equals(configuration.Feature, featureName, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void FeatureRepository_DeleteNonExisitingFeature_ShouldThrowException()
        {
            const string featureName = "kjsddfskj";
            Action action = () => _sut.Delete(featureName);

            action.Should().Throw<KeyNotFoundException>()
                .WithMessage($"Feature <{featureName}> is not configured.");
        }

        [Fact]
        public void FeatureRepository_DeleteFeature_ShouldBePersistedImmediately()
        {
            const string featureName = "OtherRoot.Font";

            _sut.Delete(featureName);

            var checkRepository = new DiskFeatureRepository(_destFileName);

            checkRepository.Select("")
                .Should().HaveCount(3)
                .And.NotContain(configuration => string.Equals(configuration.Feature, featureName, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void FeatureRepository_Update_EmptyFeatureName_ShouldThrow()
        {
            Action action = () => _sut.Update("", _fixture.Create<string>());

            action.Should().Throw<ArgumentException>()
                .Which.Message.Should().StartWith("Feature name cannot be empty");
        }

        [Fact]
        public void FeatureRepository_Update_NullFeatureName_ShouldThrow()
        {
            Action action = () => _sut.Update(null, _fixture.Create<string>());

            action.Should().Throw<ArgumentException>()
                .Which.Message.Should().StartWith("Feature name cannot be empty");
        }

        [Fact]
        public void FeatureRepository_Update_ShouldNotChangeNumberOfFeatures()
        {
            var newValue = _fixture.Create<string>();
            const string featureName = "OtherRoot.Font";

            var origFeaturesList = _sut.Select(null);

            var newFeaturesList = _sut.Update(featureName, newValue);

            newFeaturesList.Should()
                .HaveSameCount( origFeaturesList );
        }

        [Fact]
        public void FeatureRepository_Update_ShouldContainTheSameFeatures()
        {
            var newValue = _fixture.Create<string>();
            const string featureName = "OtherRoot.Font";

            var origFeaturesList = _sut.Select(null)
                .Select(feature => feature.Feature);

            var newFeaturesList = _sut.Update(featureName, newValue)
                .Select(feature => feature.Feature);

            newFeaturesList.Should()
                .BeEquivalentTo( origFeaturesList );
        }
