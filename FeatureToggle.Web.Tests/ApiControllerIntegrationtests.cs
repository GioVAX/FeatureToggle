using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FeatureToggle.Definitions;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;
using System.Net.Http;

namespace FeatureToggle.Web.Tests
{
    public class ApiControllerIntegrationTests
    {
        private readonly HttpClient _httpClient;

        public ApiControllerIntegrationTests()
        {
            const string configurationFile = "Features.json";
            RestoreFeaturesConfiguration(configurationFile);

            _httpClient = SetupHttpFramework(configurationFile);
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
        public async Task Get_All_Features_Returns_List_Of_Features_And_Http200()
        {
            // Act
            var response = await _httpClient.GetAsync("/api/Feature/GetFeatures");

            var responseContent = await response.Content.ReadAsStringAsync();
            var features = JsonConvert.DeserializeObject<IEnumerable<FeatureConfiguration>>(responseContent);

            // Assert
            features
                .Should().NotBeEmpty();
            response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }
    }
}
