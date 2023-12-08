using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel;

namespace AyanaTests
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ApplicationDbContext> _dbContextMock;
        private HomeController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContextMock = new Mock<ApplicationDbContext>();
            _controller = new HomeController(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void CategoryView_ShouldReturnFilteredProducts()
        {
            // Arrange
            var category = "Birthday";
            var flowerType = "Rose";
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product1", Category = "Birthday", FlowerType = "Rose" },
                new Product { ProductID = 2, Name = "Product2", Category = "Birthday", FlowerType = "Lily" },
                new Product { ProductID = 3, Name = "Product3", Category = "Anniversary", FlowerType = "Rose" },
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            var result = controller.CategoryView(category);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<Product>));
            var filteredProducts = (List<Product>)viewResult.Model;

            Assert.AreEqual(2, filteredProducts.Count, $"Should return 2 products for category '{category}' and flower type '{flowerType}'");
            Assert.IsTrue(filteredProducts.All(p => p.Category == category || (p.FlowerType != null && p.FlowerType == flowerType)), "All products should match the category and/or flower type");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void BestSellers_ShouldReturnTop3ProductsByPrice()
        {
            // Arrange
        var productList = new List<Product>
        {
            new Product { ProductID = 1, Name = "Product1", Price = 20.0 },
            new Product { ProductID = 2, Name = "Product2", Price = 15.0 },
            new Product { ProductID = 3, Name = "Product3", Price = 25.0 },
            new Product { ProductID = 4, Name = "Product4", Price = 18.0 },
        };

            var productDbSetMock = new Mock<DbSet<Product>>();

            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            controller.BestSellers();
            var result = controller.ViewBag.BestSellers as List<Product>;

            // Assert
            Assert.IsNotNull(result, "BestSellers should not be null");
            Assert.AreEqual(3, result.Count, "Should return top 3 best-selling products by price");
            Assert.AreEqual(25.0, result[0].Price, "First product should have the highest price");
            Assert.AreEqual(20.0, result[1].Price, "Second product should have the second-highest price");
            Assert.AreEqual(18.0, result[2].Price, "Third product should have the third-highest price");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void BirthdayBestSellers_ShouldReturnTop3BirthdayBestSellingProducts()
        {
            // Arrange
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product1", Category = "Birthday", Price = 20 },
                new Product { ProductID = 2, Name = "Product2", Category = "Birthday", Price = 15 },
                new Product { ProductID = 3, Name = "Product3", Category = "Birthday", Price = 25 },
                new Product { ProductID = 4, Name = "Product4", Category = "OtherCategory", Price = 10 },
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            controller.BirthdayBestSellers();

            // Assert
            var result = controller.ViewBag.BirthdayBestSellers as List<Product>;
            Assert.IsNotNull(result, "ViewBag.BirthdayBestSellers should not be null");
            Assert.AreEqual(3, result.Count, "Should return top 3 birthday best-selling products");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void OverallRating_ShouldCalculateAverageRating()
        {
            // Arrange
            var orderList = new List<Order>
            {
                new Order { OrderID = 1, Rating = 4 },
                new Order { OrderID = 2, Rating = 5 },
                new Order { OrderID = 3, Rating = 3 },
                new Order { OrderID = 4, Rating = null }, // Should be ignored in the calculation
            };

            var orderDbSetMock = new Mock<DbSet<Order>>();
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderList.AsQueryable().Provider);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderList.AsQueryable().Expression);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderList.AsQueryable().ElementType);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => orderList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            controller.OverallRating();

            // Assert
            var result = controller.ViewBag.rating;
            Assert.IsNotNull(result, "ViewBag.rating should not be null");
            Assert.AreEqual(4, result, "Should calculate the correct average rating");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void Category_ShouldReturnProductsInCategory()
        {
            // Arrange
            var category = "Category1";
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product1", Category = "Category1" },
                new Product { ProductID = 2, Name = "Product2", Category = "Category1" },
                new Product { ProductID = 3, Name = "Product3", Category = "OtherCategory" },
            };

            var productDbSetMock = new Mock<DbSet<Product>>();

            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            var result = controller.Category(category);

            // Assert
            Assert.IsInstanceOfType(result, typeof(List<Product>));
            var list = (List<Product>)result;
            Assert.AreEqual(2, list.Count, $"Should return 2 products in category '{category}'");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void CategoryView_ShouldReturnEmptyListForNullResults()
        {
            // Arrange
            var category = "";
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product1", Category = "Category1" },
                new Product { ProductID = 2, Name = "Product2", Category = "Category1" },
                new Product { ProductID = 3, Name = "Product3", Category = "OtherCategory" },
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            var result = controller.CategoryView(category);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsNotNull(viewResult.Model, $"Should handle null results gracefully for category '{category}'");
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<Product>), "Should set an empty list for category in the model");
            CollectionAssert.AreEqual(new List<Product>(), (List<Product>)viewResult.Model, "Should have an empty list for category in the model");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void Category_ShouldReturnZeroProductsInCategory()
        {
            // Arrange
            var category = "";
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product1", Category = "Category1" },
                new Product { ProductID = 2, Name = "Product2", Category = "Category1" },
                new Product { ProductID = 3, Name = "Product3", Category = "OtherCategory" },
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            // Act
            var result = controller.Category(category);

            // Assert
            Assert.IsInstanceOfType(result, typeof(List<Product>));
            var list = (List<Product>)result;
            Assert.AreEqual(0, list.Count, $"Should return 0 products in category '{category}'");
            Assert.AreEqual(null, controller.ViewBag.Category, "Should set ViewBag.Category to the correct category");
            Assert.IsNull(controller.ViewBag.Products, "Should set ViewBag.Products to null for 0 products");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void Privacy_ShouldReturnView()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Privacy action should return a ViewResult");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void Help_ShouldReturnView()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);

            // Act
            var result = controller.Help();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Help action should return a ViewResult");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void DeliveryPolicy_ShouldReturnView()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);

            // Act
            var result = controller.DeliveryPolicy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "DeliveryPolicy action should return a ViewResult");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void SignIn_ShouldReturnView()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);

            // Act
            var result = controller.SignIn();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "SignIn action should return a ViewResult");
        }

        // written by : Ilhan Hasičić
        [TestMethod]
        public void Subscription_ShouldReturnView()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);

            // Act
            var result = controller.Subscription();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult), "Subscription action should return a ViewResult");
        }

       
        // written by : Aida Zametica
        [TestMethod]
        public void Index_ShouldReturnView()
        {
        
            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Product1", Category = "Birthday", Price = 20 },
                new Product { ProductID = 2, Name = "Product2", Category = "Birthday", Price = 15 },
                new Product { ProductID = 3, Name = "Product3", Category = "Birthday", Price = 25 },
                new Product { ProductID = 4, Name = "Product4", Category = "OtherCategory", Price = 10 },
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => productList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);


            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        // written by : Aida Zametica
        [TestMethod]
        public void OverallRating_ShouldReturnAverageRatingZero()
        {
        
            var orderList = new List<Order>{};

            var orderDbSetMock = new Mock<DbSet<Order>>();
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderList.AsQueryable().Provider);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderList.AsQueryable().Expression);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderList.AsQueryable().ElementType);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => orderList.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);

            var controller = new HomeController(Mock.Of<ILogger<HomeController>>(), dbContextMock.Object);

            controller.OverallRating();

            var result = controller.ViewBag.rating;
  
            Assert.AreEqual(0, result);
        }

        /*
        [TestMethod]
        public void AboutUs_ShouldReturnView()
        {

            var controllerMock = new Mock<HomeController>(Mock.Of<ILogger<HomeController>>(), _dbContextMock.Object);

            controllerMock.Setup(x => x.OverallRating());


            var controller = controllerMock.Object;

            var result = controller.AboutUs();


            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }
        */



    }
}
