using Ayana;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class ProgramTests
    {
        private StringWriter consoleOutput;

        [TestInitialize]
        public void Initialize()
        {
            // Redirect console output to a StringWriter
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Reset console output
            consoleOutput.Dispose();
        }

        [TestMethod]
        public async Task Main_ShouldCallBuildAndRun()
        {
            // Arrange
            var originalOut = Console.Out;
            var args = new string[] { };

            try
            {
                // Act
                await Task.Run(() => Program.Main(args));

                // Allow some time for the asynchronous Main method to complete
                Thread.Sleep(1000);

                // Assert
                var output = consoleOutput.ToString();
                Assert.IsTrue(output.Contains("Application started."), "Build and Run methods should be called.");
            }
            finally
            {
                // Restore console output
                Console.SetOut(originalOut);
            }
        }
        [TestMethod]
        public void CreateHostBuilder_ShouldReturnValidHostBuilder()
        {
            // Arrange & Act
            var args = new string[] { };
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            Assert.IsNotNull(hostBuilder);
            Assert.IsInstanceOfType(hostBuilder, typeof(IHostBuilder));
        }
    }

}
