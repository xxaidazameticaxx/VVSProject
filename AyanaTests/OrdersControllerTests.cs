using System.Linq.Expressions;
using System.Security.Claims;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class OrdersControllerTests
    {
        private Mock<ApplicationDbContext> dbContextMock;
        private OrdersController controller;
        private List<Order> orderList;
        private DateTime deliveryDateTime = DateTime.Now.AddDays(5);
        private DateTime purchaseDateTime = DateTime.Now;
        private Mock<ClaimsPrincipal> userMock;
        private Order testOrder = new Order { OrderID = 1, CustomerID = "userId", PaymentID = 1, purchaseDate = DateTime.Now, personalMessage = "HB", IsOrderSent = true, Rating = 5, TotalAmountToPay = 100, DeliveryDate = DateTime.Now.AddDays(2) };

        [TestInitialize]
        public void TestInitialize()
        {
            orderList = new List<Order>
            {
                testOrder,
                new Order { OrderID = 2, CustomerID = "other", PaymentID = 2, purchaseDate = DateTime.Now, personalMessage = "Happy birthday!", IsOrderSent = false, Rating = null, TotalAmountToPay = 50, DeliveryDate = DateTime.Now.AddDays(3), Customer = new ApplicationUser(), Payment = new Payment()},
                new Order { OrderID = 3, DeliveryDate = DateTime.Now.AddDays(2)},
                new Order { OrderID = 7, DeliveryDate = deliveryDateTime, PaymentID = 1,purchaseDate = purchaseDateTime},

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

            var paymentList = new List<Payment>
            {
                new Payment {PaymentID = 1},
                new Payment {PaymentID = 2}
            };

            var paymentDbSetMock = new Mock<DbSet<Payment>>();
            paymentDbSetMock.As<IQueryable<Payment>>().Setup(m => m.Provider).Returns(paymentList.AsQueryable().Provider);
            paymentDbSetMock.As<IQueryable<Payment>>().Setup(m => m.Expression).Returns(paymentList.AsQueryable().Expression);
            paymentDbSetMock.As<IQueryable<Payment>>().Setup(m => m.ElementType).Returns(paymentList.AsQueryable().ElementType);
            paymentDbSetMock.As<IQueryable<Payment>>().Setup(m => m.GetEnumerator()).Returns(paymentList.GetEnumerator());

            var productList = new List<Product>
            {
                new Product { ProductID = 1, Name = "Rose", Price = 10.0, Stock = 50, Category = "Type A", Description = "Perfect Pink roses", FlowerType = "roses", ImageUrl = "", productType = "Bouquet"},
                new Product { ProductID = 2, Name = "Lily", Price = 8.0, Stock = 30, Category = "Type A", Description = "White lilies", FlowerType = "lily", ImageUrl = "", productType = "Bouquet" }
            };

            var productDbSetMock = new Mock<DbSet<Product>>();
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            var productOrderList = new List<ProductOrder>
            {
                new ProductOrder { ProductOrderID = 1, OrderID = 1, ProductID = 1, ProductQuantity = 2, Order = new Order()},
                new ProductOrder { ProductOrderID = 2, OrderID = 2, ProductID = 2, ProductQuantity = 1, Order = new Order()},
                new ProductOrder { ProductOrderID = 4, OrderID = 7, ProductID = 2, ProductQuantity = 2, Order = new Order()}
            };

            var productOrderDbSetMock = new Mock<DbSet<ProductOrder>>();
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.Provider).Returns(productOrderList.AsQueryable().Provider);
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.Expression).Returns(productOrderList.AsQueryable().Expression);
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.ElementType).Returns(productOrderList.AsQueryable().ElementType);
            productOrderDbSetMock.As<IQueryable<ProductOrder>>().Setup(m => m.GetEnumerator()).Returns(productOrderList.GetEnumerator());

            var productSalesList = new List<ProductSales>
            {
                new ProductSales { ProductSalesID = 1, ProductID = 2, SalesDate = purchaseDateTime},
                new ProductSales { ProductSalesID = 2, ProductID = 2, SalesDate = purchaseDateTime},
            };

            var productSalesDbSetMock = new Mock<DbSet<ProductSales>>();
            productSalesDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.Provider).Returns(productSalesList.AsQueryable().Provider);
            productSalesDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.Expression).Returns(productSalesList.AsQueryable().Expression);
            productSalesDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.ElementType).Returns(productSalesList.AsQueryable().ElementType);
            productSalesDbSetMock.As<IQueryable<ProductSales>>().Setup(m => m.GetEnumerator()).Returns(productSalesList.GetEnumerator());

            dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);
            dbContextMock.Setup(d => d.Products).Returns(productDbSetMock.Object);
            dbContextMock.Setup(d => d.ProductOrders).Returns(productOrderDbSetMock.Object);
            dbContextMock.Setup(d => d.ProductSales).Returns(productSalesDbSetMock.Object);
            dbContextMock.Setup(d => d.Payments).Returns(paymentDbSetMock.Object);


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
        public async Task Edit_OrderIsNull_NotFoundResult()
        {
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "other"));

            var result = await controller.Edit(new Order { OrderID = 4, Rating = null });

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
            Assert.AreEqual("UserOrders", redirectResult.ViewName);
        }

        // TDD
        [TestMethod]
        public void ActiveOrders_WhenUserIsAuthenticated_ShouldReturnViewWithUserOrdersAndProducts()
        {
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            var result = controller.ActiveOrders();

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var userOrders = viewResult.ViewData["UserOrders"] as List<Order>;
            var orderProducts = viewResult.ViewData["OrderProducts"] as List<List<Product>>;

            Assert.IsNotNull(userOrders, "UserOrders should not be null.");
            Assert.IsNotNull(orderProducts, "OrderProducts should not be null.");
            Assert.AreEqual(1, userOrders.Count, "UserOrders should have one item.");
            Assert.AreEqual(1, orderProducts.Count, "OrderProducts should have one item.");
            Assert.AreEqual("ActiveOrders", viewResult.ViewName, "The view name should be 'ActiveOrders'.");
        }

        // TDD
        [TestMethod]
        public async Task CancelOrder_DeliveryDateTooClose_ShouldSetErrorMessageAndRedirectToActiveOrders()
        {

            var orderToDelete = new Order { OrderID = 3, DeliveryDate = DateTime.Now.AddDays(2) };

            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var result = await controller.CancelOrder(orderToDelete);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = (RedirectResult)result;
            Assert.AreEqual("ActiveOrders", redirectResult.Url);

            Assert.AreEqual("You cannot cancel an order scheduled for delivery within the next 3 days.", controller.TempData["ErrorMessage"]);
        }

        // TDD
        [TestMethod]
        public async Task CancelOrder_DeliveryDateCorrect_ShouldSetSuccessMessageAndRedirectToActiveOrders()
        {

            var controller = new Mock<OrdersController>(dbContextMock.Object)
            {
                CallBase = true
            };

            userMock = new Mock<ClaimsPrincipal>();

            controller.Object.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            controller.Setup(c => c.ProcessOrderCancellationAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.Object.TempData = tempData;

            var orderToDelete = new Order { OrderID = 7, DeliveryDate = deliveryDateTime, PaymentID = 1, purchaseDate = purchaseDateTime };

            var result = await controller.Object.CancelOrder(orderToDelete);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirectResult = (RedirectResult)result;
            Assert.AreEqual("ActiveOrders", redirectResult.Url);

            Assert.AreEqual("Order successfully canceled.", controller.Object.TempData["SuccessMessage"]);
        }

        // TDD
        [TestMethod]
        public async Task CancelOrder_OrderDoesNotExist_ShouldReturnNotFound()
        {

            var nonExistingOrder = new Order { OrderID = 444 };

            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            var result = await controller.CancelOrder(nonExistingOrder);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        // TDD
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CancelOrder_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            var nonExistingOrder = new Order { OrderID = 4 };
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(a => a.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim)null);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var result = await controller.CancelOrder(nonExistingOrder);
        }

        // TDD
        [TestMethod]
        public async Task ProcessOrderCancellationAsync_ShouldCancelOrderAndAssociatedEntities()
        {

            Order orderToDelete = new Order { OrderID = 7, DeliveryDate = deliveryDateTime, PaymentID = 1, purchaseDate = purchaseDateTime };

            await controller.ProcessOrderCancellationAsync(orderToDelete);
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