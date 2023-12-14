using Ayana.Data;
using Ayana.Models;
using Microsoft.EntityFrameworkCore;

namespace AyanaTests
{
    [TestClass()]
    public class ApplicationDbContextTests
    {

        // written by : Aida Zametica
        [TestMethod]
        public void ApplicationDbContext_ModelConfiguration_ContainsEntities()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                Assert.IsTrue(context.Model.FindEntityType(typeof(Order)) != null);
                Assert.IsTrue(context.Model.FindEntityType(typeof(Discount)) != null);
                Assert.IsTrue(context.Model.FindEntityType(typeof(Payment)) != null);
                Assert.IsTrue(context.Model.FindEntityType(typeof(Cart)) != null);
                Assert.IsTrue(context.Model.FindEntityType(typeof(Product)) != null);
                Assert.IsTrue(context.Model.FindEntityType(typeof(Report)) != null);
            }
        }

        // written by : Aida Zametica
        [TestMethod]
        public void ApplicationDbContext_TableNameConfiguration_TableNamesMatchExpected()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                Assert.AreEqual("Orders", context.Model.FindEntityType(typeof(Order)).GetTableName());
                Assert.AreEqual("Discounts", context.Model.FindEntityType(typeof(Discount)).GetTableName());
                Assert.AreEqual("Payments", context.Model.FindEntityType(typeof(Payment)).GetTableName());
                Assert.AreEqual("Carts", context.Model.FindEntityType(typeof(Cart)).GetTableName());
                Assert.AreEqual("Products", context.Model.FindEntityType(typeof(Product)).GetTableName());
                Assert.AreEqual("Reports", context.Model.FindEntityType(typeof(Report)).GetTableName());
            }
        }
    }
}