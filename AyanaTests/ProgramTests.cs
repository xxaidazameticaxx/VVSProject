using Ayana;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class ProgramTests
    {
        // written by: Hasičić Ilhan
        [TestMethod]
        public void CreateHostBuilder_ShouldReturnValidHostBuilder()
        {
            var args = new string[] { };
            var hostBuilder = Program.CreateHostBuilder(args);

            Assert.IsNotNull(hostBuilder);
            Assert.IsInstanceOfType(hostBuilder, typeof(IHostBuilder));
        }
    }

}
