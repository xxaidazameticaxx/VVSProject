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
        private ApplicationDbContext _dbContext;
        private OrdersController controller;
        private List<Order> orderList;
        private Mock<ClaimsPrincipal> userMock;
        private Order testOrder;
        private int newRating = 5;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
               .Options;

            _dbContext = new ApplicationDbContext(options);

            var userId = "userId";
            var otherUserId = "other";

            testOrder = new Order { OrderID = 1, CustomerID = userId, PaymentID = 1, purchaseDate = DateTime.Now, personalMessage = null, IsOrderSent = true, Rating = null, TotalAmountToPay = 100, DeliveryDate = DateTime.Now.AddDays(2) };
            orderList = new List<Order>
            {
                testOrder,
                new Order{OrderID = 2, CustomerID = otherUserId, PaymentID = 2, purchaseDate = DateTime.Now, personalMessage = "Happy birthday!", IsOrderSent = true, Rating = null, TotalAmountToPay = 50, DeliveryDate = DateTime.Now.AddDays(3)},

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
                new Product { ProductID = 1, Name = "Rose", Price = 10.0, Stock = 50 },
                new Product { ProductID = 2, Name = "Lily", Price = 8.0, Stock = 30 }
            };

            var productOrderList = new List<ProductOrder>
            {
                new ProductOrder { ProductOrderID = 1, OrderID = 1, ProductID = 1, ProductQuantity = 2 , Order = null},
                new ProductOrder { ProductOrderID = 2, OrderID = 2, ProductID = 2, ProductQuantity = 1 , Order = null}
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

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);
            dbContextMock.Setup(d => d.ProductOrders).Returns(productOrderDbSetMock.Object);


            dbContextMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
              .Callback(async (CancellationToken cancellationToken) =>
              {
                  foreach (var order in orderList)
                  {
                      if (order.OrderID == 1)
                      {
                          order.Rating = newRating;
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

        [TestMethod]
        public async Task Edit_OrderIsNull_NotFoundResult()
        {
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "other"));

            var result = await controller.Edit(new Order { OrderID = 3, Rating = null });

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_OrderIsNotNull_OrderRatingUpdated()
        {
           
            var result = await controller.Edit(testOrder);

            var updatedOrder = orderList.FirstOrDefault(o => o.OrderID == 1);

            Assert.IsNotNull(updatedOrder, "The order should be updated.");
            Assert.AreEqual(newRating, updatedOrder.Rating, "The order rating should be updated to 5.");
        }
        

        [TestCleanup]
        public async Task TestCleanup()
        {
            await _dbContext.Database.EnsureDeletedAsync();
        }
    }
}
