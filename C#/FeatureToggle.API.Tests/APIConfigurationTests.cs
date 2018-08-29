﻿using FeatureToggle.DAL;
using FeatureToggle.Definitions;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace FeatureToggle.API.Tests
{
    public class ApiConfigurationTests
    {
        private readonly IWebHost _sut;

        public ApiConfigurationTests()
        {
            _sut = Program.BuildWebHost(null);
        }

        [Fact]
        public void FeatureRepository_ShouldBeRegisteredInDI()
        {
            var repository = _sut.Services.GetService<IFeatureRepository>();

            repository.Should().NotBeNull()
                .And.BeOfType<DiskFeatureRepository>();
        }

        [Fact]
        public void FeaturesFileConfiguration_ShouldBeRegisteredInDI()
        {
            var options = _sut.Services.GetService<IOptions<FeaturesFileConfiguration>>();

            options.Should().NotBeNull();

            options.Value.Should().NotBeNull();
        }

        [Fact]
        public void ILoggerT_ShouldBeRegisteredInDI()
        {
            var logger = _sut.Services.GetService<ILogger<ApiConfigurationTests>>();

            logger.Should().NotBeNull();
        }
    }
}
