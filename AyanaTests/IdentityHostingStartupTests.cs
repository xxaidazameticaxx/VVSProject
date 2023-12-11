using Ayana.Areas.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class IdentityHostingStartupTests
    {
        // written by: Hasičić Ilhan
        [TestMethod]
        public void Configure_ShouldCallConfigureServices()
        {
            var hostingEnvironmentMock = new Mock<IWebHostEnvironment>();
            var configurationMock = new Mock<IConfiguration>();

            var builderMock = new Mock<IWebHostBuilder>();
            builderMock.Setup(b => b.ConfigureServices(It.IsAny<Action<WebHostBuilderContext, IServiceCollection>>()))
                       .Callback<Action<WebHostBuilderContext, IServiceCollection>>(action => action(null, new ServiceCollection()));

            var identityHostingStartup = new IdentityHostingStartup();

            identityHostingStartup.Configure(builderMock.Object);

            builderMock.Verify(b => b.ConfigureServices(It.IsAny<Action<WebHostBuilderContext, IServiceCollection>>()), Times.Once);
        }
    }
}
