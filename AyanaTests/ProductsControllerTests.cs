using System.Linq.Expressions;
using System.Security.Claims;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Patterns;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<IProduct> iProductMock;
        private Mock<ApplicationDbContext> dbContextMock;
        private ProductsController controller;
        private List<Product> testData;

        [TestInitialize]
        public void TestInitialize()
        {
            iProductMock = new Mock<IProduct>();
            dbContextMock = new Mock<ApplicationDbContext>();

            testData = new List<Product>
        {
            new Product
            {
                ProductID = 1,
                Name = "Product 1",
                ImageUrl = "url1",
                Price = 19.99,
                FlowerType = "Rose",
                Stock = 10,
                Category = "Flowers",
                Description = "Beautiful red rose",
                productType = "Type A"
            },
            new Product
            {
                ProductID = 2,
                Name = "Product 2",
                ImageUrl = "url2",
                Price = 29.99,
                FlowerType = "Lily",
                Stock = 15,
                Category = "Flowers",
                Description = "Elegant white lily",
                productType = "Type B"
            }
        };

            // Konfiguracija DbContext mock-a
            var productsDbSetMock = new Mock<DbSet<Product>>();
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(testData.AsQueryable().Provider);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(testData.AsQueryable().Expression);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(testData.AsQueryable().ElementType);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            productsDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(async keyValues =>
                {
                    var id = (int)keyValues[0];
                    return await Task.FromResult(testData.FirstOrDefault(o => o.ProductID == id));
                });

            dbContextMock.Setup(d => d.Products).Returns(productsDbSetMock.Object);

            

            controller = new ProductsController(dbContextMock.Object, iProductMock.Object);
        }


        [TestMethod]
        public async Task Index_ReturnsViewWithProducts()
        {
            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Create_ReturnsView()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public void SearchResult_ReturnsViewWithFilteredProducts()
        {
            // Arrange
            var searchString = "Product 1"; // Postavite željenu pretragu
            var expectedResultCount = 1; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.SearchResult(searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void SearchResult_ReturnsViewWithAllProductsWhenSearchIsNull()
        {
            // Act
            var result = controller.SearchResult(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(testData, model);
        }

        [TestMethod]
        public async Task Details_ReturnsViewWithProduct()
        {
            int productId = 1; // Postavite ID proizvoda koji želite koristiti za testiranje

            // Act
            var result = await controller.Details(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Dobijanje modela iz rezultata
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Product;

            // Assert za model
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.ProductID);
        }






    }


}
