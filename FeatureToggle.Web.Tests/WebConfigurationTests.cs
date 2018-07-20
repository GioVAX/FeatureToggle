using FeatureToggle.DAL;
using FeatureToggle.Definitions;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FeatureToggle.Web.Tests
{
    public class WebConfigurationTests
    {
        private readonly IWebHost _sut;

        public WebConfigurationTests()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(cfg => cfg[It.IsAny<string>()])
                .Returns(null as string);

            _sut = Program.BuildWebHost(null, configMock.Object);
        }

        [Fact]
        public void FeatureRepository_ShouldBeRegisteredInDI()
        {
            var repository = _sut.Services.GetService<IFeatureRepository>();

            repository.Should().NotBeNull()
                .And.BeOfType<DiskFeatureRepository>();
        }

        [Fact]
        public void FeatureConfigurationFile_ShouldBeRegisteredInDI()
        {
            var options = _sut.Services.GetService<IOptions<FeaturesFileConfiguration>>();

            options.Should().NotBeNull();

            options.Value.Should().NotBeNull();
        }

        [Fact]
        public void ILoggerT_ShouldBeRegisteredInDI()
        {
            var logger = _sut.Services.GetService<ILogger<WebConfigurationTests>>();

            logger.Should().NotBeNull();
        }
    }
}
