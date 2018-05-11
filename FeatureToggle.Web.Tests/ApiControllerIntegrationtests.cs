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

namespace FeatureToggle.Web.Tests
{
    public class ApiControllerIntegrationTests : IDisposable
    {
        private readonly string _configurationFile;
        private readonly HttpClient _httpClient;
        private readonly Fixture _fixture;

        public ApiControllerIntegrationTests()
        {
            _fixture = new Fixture();

            _configurationFile = Path.ChangeExtension(_fixture.Create<string>(), "json");

            RestoreFeaturesConfiguration(_configurationFile);
            _httpClient = SetupHttpFramework(_configurationFile);
        }

        public void Dispose()
        {
            File.Delete(_configurationFile);
        }

        private HttpClient SetupHttpFramework(string configurationFile)
        {
            // using WebHost.CreateDefauldBuilder so that the json configuration files are picked up.
            // WebHostBuilder() ignores them by default unless added explicitly. 
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseSetting("FeaturesConfigurationFile", configurationFile);

            var testServer = new TestServer(builder);
            return testServer.CreateClient();
        }

        private void RestoreFeaturesConfiguration(string destinationFile) =>
            File.Copy("Features_Test.json", destinationFile, true);

        [Fact]
        public async void Get_All_Features_Returns_List_Of_Features_And_Http200()
        {
            // Act
            var response = await _httpClient.GetAsync("/api/Feature/GetFeatures");

            var responseContent = await response.Content.ReadAsStringAsync();
            var features = JsonConvert.DeserializeObject<IEnumerable<FeatureConfiguration>>(responseContent);

            // Assert
            features
                .Should().NotBeNull()
                .And.NotBeEmpty();

            response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void GetFeature_WithParameter_ReturnsFilteredList_AndHttpStatus200()
        {
            const string pattern = "FeatureToggle";
            var response = await _httpClient.GetAsync($"/api/Feature/GetFeatures?beginningWith={pattern}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var features = JsonConvert.DeserializeObject<IEnumerable<FeatureConfiguration>>(responseContent);

            features
                .Should().NotBeNull()
                .And.HaveCount(2)
                .And.OnlyContain(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));

            response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }
    }
}
