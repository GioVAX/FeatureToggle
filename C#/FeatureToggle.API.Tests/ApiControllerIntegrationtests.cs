using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FeatureToggle.Definitions;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace FeatureToggle.API.Tests
{
    public class ApiControllerIntegrationTests : IDisposable
    {
        private readonly string _configurationFile;
        private readonly HttpClient _httpClient;

        public ApiControllerIntegrationTests()
        {
            _configurationFile = Path.ChangeExtension(new Fixture().Create<string>(), "json");

            RestoreFeaturesConfiguration(_configurationFile);
            _httpClient = SetupHttpFramework(_configurationFile);
        }

        public void Dispose()
        {
            File.Delete(_configurationFile);
        }

        private HttpClient SetupHttpFramework(string configurationFile)
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseSetting("FeaturesConfigurationFile", configurationFile);

            var testServer = new TestServer(builder);
            return testServer.CreateClient();
        }

        private void RestoreFeaturesConfiguration(string destinationFile) =>
            File.Copy("Features_Test.json", destinationFile, true);

        [Fact]
        public async void GetAllFeatures_ShouldReturnListOfFeatures_AndHttpStatus200()
        {
            // Act
            var response = await _httpClient.GetAsync("/GetFeatures");
            var features = await ExtractFeaturesFromResponse(response);

            // Assert
            features
                .Should().NotBeNull()
                .And.NotBeEmpty();

            response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void GetFeatureWithParameter_ShouldReturnFilteredList_AndHttpStatus200()
        {
            const string pattern = "FeatureToggle";

            var response = await _httpClient.GetAsync($"/GetFeatures?beginningWith={pattern}");
            var features = await ExtractFeaturesFromResponse(response);

            features
                .Should().NotBeNull()
                .And.HaveCount(2)
                .And.OnlyContain(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));

            response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }

        private async Task<IEnumerable<FeatureConfiguration>> ExtractFeaturesFromResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<FeatureConfiguration>>(responseContent);
        }
    }
}
