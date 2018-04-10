using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using FeatureToggle.Definitions;
using FeatureToggle.DAL;
using Microsoft.Extensions.Configuration;

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
            var repository = _sut.Services.GetService(typeof(IFeatureRepository));

            repository.Should().NotBeNull()
                .And.BeOfType<DiskFeatureRepository>();
        }

        //[Fact]
        //public void FeatureConfigurationFile_IsConfigured()
        //{
        //    var configuration = (IConfiguration )_sut.Services.GetService(typeof(IConfiguration));

        //    configuration.GetValue<string>("FeaturesConfigurationFile")
        //        .Should().NotBeNullOrWhiteSpace();
        //}
    }
}
