using System.Linq.Expressions;
using System.Security.Claims;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class OrdersControllerTests
    {
        private OrdersController controller;
        private List<Order> orderList;
        private Mock<ClaimsPrincipal> userMock;
        private Order testOrder = new Order { OrderID = 1, CustomerID = "userId", PaymentID = 1, purchaseDate = DateTime.Now, personalMessage = "HB", IsOrderSent = true, Rating = 5, TotalAmountToPay = 100, DeliveryDate = DateTime.Now.AddDays(2) };
        private Mock<ApplicationDbContext> dbContextMock;


        [TestInitialize]
        public void TestInitialize()
        {
            orderList = new List<Order>
            {
                testOrder,
                new Order{OrderID = 2, CustomerID = "other", PaymentID = 2, purchaseDate = DateTime.Now, personalMessage = "Happy birthday!", IsOrderSent = false, Rating = null, TotalAmountToPay = 50, DeliveryDate = DateTime.Now.AddDays(3), Customer = new ApplicationUser(), Payment = new Payment()},

            };

            var orderDbSetMock = new Mock<DbSet<Order>>();
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderList.AsQueryable().Provider);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderList.AsQueryable().Expression);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderList.AsQueryable().ElementType);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orderList.GetEnumerator());

            orderDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>()))
               .Returns<object[]>(async keyValues =>
               {
                   var id = (int)keyValues[0];
                   return await Task.FromResult(orderList.FirstOrDefault(o => o.OrderID == id));
               });



            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Rose", Price = 10.0, Stock = 50, Category = "Type A", Description = "Perfect Pink roses", FlowerType = "roses", ImageUrl = "", productType = "Bouquet"},
                new Product { ProductID = 2, Name = "Lily", Price = 8.0, Stock = 30, Category = "Type A", Description = "White lilies", FlowerType = "lily", ImageUrl = "", productType = "Bouquet" }
            };

            var productOrderList = new List<ProductOrder>
            {
                new ProductOrder { ProductOrderID = 1, OrderID = 1, ProductID = 1, ProductQuantity = 2, Order = new Order() },
                new ProductOrder { ProductOrderID = 2, OrderID = 2, ProductID = 2, ProductQuantity = 1, Order= new Order()}
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            var productOrderDbSetMock = new Mock<DbSet<ProductOrder>>();
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.Provider).Returns(productOrderList.AsQueryable().Provider);
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.Expression).Returns(productOrderList.AsQueryable().Expression);
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.ElementType).Returns(productOrderList.AsQueryable().ElementType);
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.GetEnumerator()).Returns(productOrderList.GetEnumerator());

            dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);
            dbContextMock.Setup(d => d.ProductOrders).Returns(productOrderDbSetMock.Object);
           

            dbContextMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
              .Callback(async (CancellationToken cancellationToken) =>
              {
                  foreach (var order in orderList)
                  {
                      if (order.OrderID == testOrder.OrderID)
                      {
                          order.Rating = testOrder.Rating;
                      }
                  }
              })
              .Returns(Task.FromResult(0));

            controller = new OrdersController(dbContextMock.Object);

            userMock = new Mock<ClaimsPrincipal>();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void GetOrderProducts_OrderIsNotNull_ShouldOpenViewWithCorrectUserOrders()
        {
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            var result = controller.UserOrders();

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var redirectResult = (ViewResult)result;
            var userOrders = redirectResult.ViewData["userOrders"] as List<Order>;
            var orderProducts = redirectResult.ViewData["orderProducts"] as List<List<Product>>;

            Assert.AreEqual(1, userOrders?.Count, "UserOrders should have one item.");
            Assert.AreEqual(1, orderProducts?.Count, "OrderProducts should have one item.");
            Assert.AreEqual("UserOrders", redirectResult.ViewName, "The view name should be 'UserOrders'.");
        }

        // written by : Lejla Heleg
        [TestMethod]
        public async Task Edit_OrderIsNull_NotFoundResult()
        {
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "other"));

            var result = await controller.Edit(new Order { OrderID = 3, Rating = null });

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        // written by : Lejla Heleg
        [TestMethod]
        public async Task Edit_OrderIsNotNull_OrderRatingUpdated()
        {
            var result = await controller.Edit(testOrder);

            var updatedOrder = orderList.FirstOrDefault(o => o.OrderID == 1);

            Assert.IsNotNull(updatedOrder, "The order should be updated.");
            Assert.AreEqual(testOrder.Rating, updatedOrder.Rating, "The order rating should be updated to 5.");
        }


        [TestMethod]
        public void ActiveOrders_ShouldReturnViewWithCorrectData()
        {
            var result = controller.ActiveOrders() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ActiveOrders", result.ViewName);
        }

        [TestMethod]
        public async Task CancelOrder_OrderDoesNotExist_ShouldReturnNotFound()
        {
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            var result = await controller.CancelOrder(new Order { OrderID = 158 }) as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetUserOrders_ReturnsUserSpecificOrders()
        {
            string userId = "userId";
            var today = DateTime.Today;
            var expectedUserOrders = new List<Order>
            {
                new Order { OrderID = 1, CustomerID = userId, DeliveryDate = today.AddDays(1) },
                new Order { OrderID = 2, CustomerID = userId, DeliveryDate = today.AddDays(2) },
            };
            
            var orderDbSetMock = new Mock<DbSet<Order>>();
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(expectedUserOrders.AsQueryable().Provider);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(expectedUserOrders.AsQueryable().Expression);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(expectedUserOrders.AsQueryable().ElementType);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(expectedUserOrders.GetEnumerator());

            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);

            var result = controller.GetUserOrders(userId);

            CollectionAssert.AreEqual(expectedUserOrders, result.ToList());
        }

    }
}