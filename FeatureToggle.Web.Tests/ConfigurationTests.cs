using FeatureToggle.DAL;
using FeatureToggle.Definitions;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeatureToggle.Web.Tests
{
    public class ConfigurationTests
    {
        readonly IWebHost _sut;

        public ConfigurationTests()
        {
            _sut = WebHost.CreateDefaultBuilder()
               .UseStartup<Startup>()
               .Build();
        }

        [Fact]
        public void FeatureRepository_ShouldBeRegistered()
        {
            var repository = _sut.Services.GetService<IFeatureRepository>();

            repository.Should().NotBeNull()
                .And.BeOfType<DiskFeatureRepository>();
        }

        //[Fact]
        //public void FeatureConfigurationFile_IsConfigured()
        //{
        //    var configuration = (IConfiguration )_sut.Services.GetService<IFeatureRepository>();

        //    configuration.GetValue<string>("FeaturesConfigurationFile")
        //        .Should().NotBeNullOrWhiteSpace();
        //}
    }
}
