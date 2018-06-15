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
        public void DiskFeatureRepository_ShouldImplementIFeatureRepository()
        {
            _sut.Should().BeAssignableTo<IFeatureRepository>();
        }

        [Fact]
        public void DiskFeatureRepository_InitWithNullString_ShouldReturnEmptyListOfFeatures()
        {
            var sut = new DiskFeatureRepository(null);

            sut.Select("").Should().BeEmpty();
        }

        [Fact]
        public void DiskFeatureRepository_InitWithNonExistingFile_ShouldReturnEmptyListOfFeatures()
        {
            var sut = new DiskFeatureRepository("lksflskdjf.oiwefiuwrf");

            sut.Select("").Should().BeEmpty();
        }

        [Fact]
        public void DiskFeatureRepository_Select_ShouldReturnListOfFeatures()
        {
            _sut.Select("").Should()
                .BeOfType<List<FeatureConfiguration>>();
        }

        [Fact]
        public void DiskFeatureRepository_SelectNoPattern_ShouldReturn4ValidFeatures()
        {
            var features = _sut.Select("").ToList();
            using (new AssertionScope())
            {
                features.Should().HaveCount(4);
                features.Should().OnlyContain(fc => !(string.IsNullOrEmpty(fc.Feature) || string.IsNullOrEmpty(fc.Value)));
            }
        }

        [Fact]
        public void DiskFeatureRepository_SelectPatternFeatureToggle_ShouldReturn2FeaturesStartingWithFeatureToggle()
        {
            const string pattern = "FeatureToggle";
            var features = _sut.Select(pattern);

            features.Should().HaveCount(2)
                .And.OnlyContain(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void DiskFeatureRepository_DeleteExisitingFeature_ShouldRemoveTheFeature()
        {
            const string featureName = "OtherRoot.Font";

            _sut.Delete(featureName);

            _sut.Select("")
                .Should().HaveCount(3)
                .And.NotContain(configuration => string.Equals(configuration.Feature, featureName, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void DiskFeatureRepository_DeleteNonExisitingFeature_ShouldThrowException()
        {
            const string featureName = "kjsddfskj";
            Action action = () => _sut.Delete(featureName);

            action.Should().Throw<KeyNotFoundException>()
                .WithMessage($"Feature <{featureName}> is not configured.");
        }

        [Fact]
        public void DiskFeatureRepository_DeleteFeature_ShouldBePersistedImmediately()
        {
            const string featureName = "OtherRoot.Font";

            _sut.Delete(featureName);

            var checkRepository = new DiskFeatureRepository(_destFileName);

            checkRepository.Select("")
                .Should().HaveCount(3)
                .And.NotContain(configuration => string.Equals(configuration.Feature, featureName, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void DiskFeatureRepository_Update_EmptyFeatureName_ShouldThrow()
        {
            Action action = () => _sut.Update("", _fixture.Create<string>());

            action.Should().Throw<ArgumentException>()
                .Which.Message.Should().StartWith("Feature name cannot be empty");
        }

        [Fact]
        public void DiskFeatureRepository_Update_NullFeatureName_ShouldThrow()
        {
            Action action = () => _sut.Update(null, _fixture.Create<string>());

            action.Should().Throw<ArgumentException>()
                .Which.Message.Should().StartWith("Feature name cannot be empty");
        }

        [Fact]
        public void DiskFeatureRepository_Update_ShouldNotChangeNumberOfFeatures()
        {
            var newValue = _fixture.Create<string>();
            const string featureName = "OtherRoot.Font";

            var origFeaturesList = _sut.Select(null);

            var newFeaturesList = _sut.Update(featureName, newValue);

            newFeaturesList.Should()
                .HaveSameCount(origFeaturesList);
        }

        [Fact]
        public void DiskFeatureRepository_Update_ShouldContainTheSameFeatures()
        {
            var newValue = _fixture.Create<string>();
            const string featureName = "OtherRoot.Font";

            var origFeaturesList = _sut.Select(null)
                .Select(feature => feature.Feature);

            var newFeaturesList = _sut.Update(featureName, newValue)
                .Select(feature => feature.Feature);

            newFeaturesList.Should()
                .BeEquivalentTo(origFeaturesList);
        }

        [Fact]
        public void DiskFeatureRepository_Update_ShouldChangeFeatureToTheNewValue()
        {
            var newValue = _fixture.Create<string>();
            const string featureName = "OtherRoot.Font";

            var newFeaturesList = _sut.Update(featureName, newValue);

            var newFeature = newFeaturesList.Single(feature =>
                string.Equals(feature.Feature, featureName, StringComparison.InvariantCultureIgnoreCase));

            newFeature.Value
                .Should().Be(newValue);
        }


        [Fact]
        public void DiskFeatureRepository_UpdateUnknownFeature_ShouldThrowShowingFeatureName()
        {
            var featureName = _fixture.Create<string>();

            Action action = () => _sut.Update(featureName, _fixture.Create<string>());

            action.Should().Throw<KeyNotFoundException>()
                .Which.Message.Should().StartWith($"Feature <{featureName}> is not configured");
        }

        [Fact]
        public void DiskFeatureRepository_Update_ShouldBePersisted()
        {
            const string featureName = "OtherRoot.Font";
            var newValue = _fixture.Create<string>();

            _sut.Update(featureName, newValue);

            var checkRepository = new DiskFeatureRepository(_destFileName);

            var feature = checkRepository.Select(featureName).Single();

            feature.Value.Should().Be(newValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DiskFeatureRepository_Add_InvalidFeatureName_ShouldThrow(string featureName)
        {
            Action action = () => _sut.Add(featureName, _fixture.Create<string>());

            action.Should().Throw<InvalidDataException>()
                .Which.Message.Should().Contain("parameter <featureName> cannot be empty");
        }

        [Fact]
        public void DiskFeatureRepository_Add_ShouldAddOneNewFeature()
        {
            var startingCount = _sut.Select("").Count();
            var newFeature = _fixture.Create<string>();
            var newValue = _fixture.Create<string>();

            _sut.Add(newFeature, newValue);

            _sut.Select("").Should().HaveCount(startingCount + 1);
        }

        [Fact]
        public void DiskFeatureRepository_Add_ShouldAddTheNewFeature()
        {
            var newFeature = _fixture.Create<string>();
            var newValue = _fixture.Create<string>();

            _sut.Add(newFeature, newValue);

            _sut.Select("")
                .Should().ContainSingle(fc => fc.Feature == newFeature)
                .Which.Value.Should().Be(newValue);
        }

        [Fact]
        public void DiskFeatureRepository_Add_ShouldBePersisted()
        {
            var newFeature = _fixture.Create<string>();
            var newValue = _fixture.Create<string>();

            _sut.Add(newFeature, newValue);

            var checkRepository = new DiskFeatureRepository(_destFileName);
            var feature = checkRepository.Select(newFeature).Single();
            feature.Value.Should().Be(newValue);
        }
    }
}
