using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class ReportsPatternTests
    {
        private Mock<ApplicationDbContext> dbContextMock;
        private ReportFactory reportFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Rose", Price = 10.0, Stock = 50, Category = "Type A", Description = "Perfect Pink roses", FlowerType = "roses", ImageUrl = "", productType = "Bouquet"},
                new Product { ProductID = 2, Name = "Tulip", Price = 15.0, Stock = 40, Category = "Type A", Description = "Elegant tulips", FlowerType = "tulips", ImageUrl = "", productType = "Bouquet"},
                new Product { ProductID = 3, Name = "Lily", Price = 8.0, Stock = 30, Category = "Type A", Description = "White lilies", FlowerType = "lily", ImageUrl = "", productType = "Bouquet" },
            };

            var productSalesList = new List<ProductSales>
            {
                new ProductSales {ProductSalesID = 1, SalesDate = DateTime.Now.AddDays(-3), ProductID = 1,
                Product = new Product() }
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            var productSaleDbSetMock = new Mock<DbSet<ProductSales>>();
            productSaleDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.Provider).Returns(productSalesList.AsQueryable().Provider);
            productSaleDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.Expression).Returns(productSalesList.AsQueryable().Expression);
            productSaleDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.ElementType).Returns(productSalesList.AsQueryable().ElementType);
            productSaleDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.GetEnumerator()).Returns(productSalesList.GetEnumerator());

            dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);
            dbContextMock.Setup(d => d.ProductSales).Returns(productSaleDbSetMock.Object);

            reportFactory = new ReportFactory(dbContextMock.Object);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void CreateReport_InvalidParameter_ThrowsArgumentException()
        {
            var reportFactory = new ReportFactory(dbContextMock.Object);

            Assert.ThrowsException<ArgumentException>(() => reportFactory.CreateReport(""));
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void GenerateReport_WeeklyReport_ReturnsByteResult()
        {
            IReport weeklyReport = reportFactory.CreateReport("weekly");

            var result = weeklyReport.GenerateReport();

            Assert.IsInstanceOfType(result, typeof(System.Byte[]));
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void GenerateReport_MonthlyReport_ReturnsByteResult()
        {

            IReport weeklyReport = reportFactory.CreateReport("monthly");

            var result = weeklyReport.GenerateReport();

            Assert.IsInstanceOfType(result, typeof(System.Byte[]));
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void GenerateReport_YearlyReport_ReturnsByteResult()
        {
            IReport weeklyReport = reportFactory.CreateReport("yearly");

            var result = weeklyReport.GenerateReport();

            Assert.IsInstanceOfType(result, typeof(System.Byte[]));
        }
    }
}
