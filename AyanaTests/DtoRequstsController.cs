using Microsoft.EntityFrameworkCore;
using Ayana.Data;
using Moq;
using Ayana.Paterni;
using Ayana.Controllers;

namespace AyanaTests
{
    [TestClass]
    public class DtoRequestsControllerTests

    {

        // written by: Aida Zametica
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task RemoveItem_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            // Create DbContextOptions for in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create an instance of ApplicationDbContext using the in-memory database options
            var dbContext = new ApplicationDbContext(options);

            var mockDiscountCodeVerifier = new Mock<IDiscountCodeVerifier>();

            var controller = new DtoRequestsController(dbContext, mockDiscountCodeVerifier.Object);

            // Act and Assert
            await controller.RemoveItem(0);
        }

    }
}
