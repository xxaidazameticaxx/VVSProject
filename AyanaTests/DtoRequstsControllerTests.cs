using Microsoft.EntityFrameworkCore;
using Moq;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace AyanaTests
{

    [TestClass]
    public class DtoRequestsControllerTests
    {
        private ApplicationDbContext _dbContext;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;

            _dbContext = new ApplicationDbContext(options);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task RemoveItem_WhenProductQuantityMoreThenOne_RemoveProduct()
        {
    
            var userId = "testUserId";
            var id = 1;

            var cartList = new List<Cart>
            {
                new Cart { CustomerID = userId, ProductID = 1, ProductQuantity = 1 }, 
                new Cart { CustomerID = "otherUserId", ProductID = 2, ProductQuantity = 1 },
            };

            var cartDbSetMock = new Mock<DbSet<Cart>>();
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartList.AsQueryable().Provider);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartList.AsQueryable().Expression);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartList.AsQueryable().ElementType);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartList.GetEnumerator());

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Cart).Returns(cartDbSetMock.Object);

            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "testUserId"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            dbContextMock.
                Setup(m => m.Remove(It.IsAny<Cart>())).Callback<Cart>((entity) => cartList.Remove(entity));

            var result = await controller.RemoveItem(1);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Cart", redirectResult.ActionName);
            Assert.AreEqual(0, redirectResult.RouteValues["discountAmount"]);
            Assert.AreEqual(1, redirectResult.RouteValues["discountType"]);
            Assert.AreEqual("", redirectResult.RouteValues["discountCode"]);

            var removedCartItem = cartList.SingleOrDefault(c => c.CustomerID == userId && c.ProductID == id);
            Assert.IsNull(removedCartItem, "The item should have been removed");

        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task RemoveItem_WhenProductQuantityEqualOne_DecreasesQuantity()
        {

            var userId = "testUserId";
            var id = 1;

            var cartList = new List<Cart>
            {
                new Cart { CustomerID = userId, ProductID = 1, ProductQuantity = 2 },
                new Cart { CustomerID = "otherUserId", ProductID = 2, ProductQuantity = 1 },
            };

            var cartDbSetMock = new Mock<DbSet<Cart>>();
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartList.AsQueryable().Provider);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartList.AsQueryable().Expression);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartList.AsQueryable().ElementType);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartList.GetEnumerator());

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Cart).Returns(cartDbSetMock.Object);

            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "testUserId"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            var result = await controller.RemoveItem(1);

            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.RedirectToActionResult));

            var redirectResult = (Microsoft.AspNetCore.Mvc.RedirectToActionResult)result;
            Assert.AreEqual("Cart", redirectResult.ActionName);
            Assert.AreEqual(0, redirectResult.RouteValues["discountAmount"]);
            Assert.AreEqual(1, redirectResult.RouteValues["discountType"]);
            Assert.AreEqual("", redirectResult.RouteValues["discountCode"]);

            // verify that the correct item was removed
            var removedCartItem = cartList.SingleOrDefault(c => c.CustomerID == userId && c.ProductID == id);
            Assert.AreEqual(1, removedCartItem?.ProductQuantity, "The item quantity should have been decreased in the cart.");

        }


        // written by : Aida Zametica
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task RemoveItem_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            var mockDiscountCodeVerifier = new Mock<IDiscountCodeVerifier>();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(a => a.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim)null);

            var controller = new DtoRequestsController(_dbContext, mockDiscountCodeVerifier.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
                }
            };

            await controller.RemoveItem(0);
        }

        // written by : Aida Zametica
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public async Task ApplyDiscount_ValidCodeAndNotExpired_ShouldRedirectToCartWithDiscount(DiscountType discountType)
        {

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            discountCodeVerifierMock.Setup(x => x.VerifyDiscountCode("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.VerifyExperationDate("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.GetDiscount("ValidDiscountCode")).Returns(new Discount
            {
                DiscountAmount = 10,
                DiscountType = discountType
            });

            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.ApplyDiscount("ValidDiscountCode") as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("ValidDiscountCode", result.RouteValues["discountCode"]);
            Assert.AreEqual("Cart", result.ActionName);
        }


        // written by : Aida Zametica
        [TestMethod]
        public async Task ApplyDiscount_ValidCodeAndExpired_ShouldRedirectToCartWithoutDiscount()
        {

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            discountCodeVerifierMock.Setup(x => x.VerifyDiscountCode("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.VerifyExperationDate("ValidDiscountCode")).Returns(false);
            discountCodeVerifierMock.Setup(x => x.GetDiscount("ValidDiscountCode")).Returns(new Discount
            {
                DiscountAmount = 10,
                DiscountType = DiscountType.AmountOff
            });

            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.ApplyDiscount("ValidDiscountCode") as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("Code is expired...", result.RouteValues["discountCode"]);
            Assert.AreEqual("Cart", result.ActionName);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task ApplyDiscount_InvalidCode_ShouldRedirectToCartWithoutDiscount()
        {

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            discountCodeVerifierMock.Setup(x => x.VerifyDiscountCode("ValidDiscountCode")).Returns(false);
            discountCodeVerifierMock.Setup(x => x.GetDiscount("ValidDiscountCode")).Returns(new Discount
            {
                DiscountAmount = 10,
                DiscountType = DiscountType.AmountOff
            });

            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.ApplyDiscount("ValidDiscountCode") as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("Wrong code, try again...", result.RouteValues["discountCode"]);
            Assert.AreEqual("Cart", result.ActionName);
        }

        // written by : Aida Zametica
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public async Task CalculateDiscount_ValidDiscountAndNotExpired_ShouldApplyDiscountCorrectly(DiscountType discountType)
        {
            var payment = new Payment
            {
                PayedAmount = 100
            };

            var discount = new Discount
            {
                DiscountCode = "ValidDiscountCode"
            };

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            discountCodeVerifierMock.Setup(x => x.VerifyDiscountCode("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.VerifyExperationDate("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.GetDiscount("ValidDiscountCode")).Returns(new Discount
            {
                DiscountID = 1,
                DiscountAmount = 10,
                DiscountType = discountType
            });
            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.CalculateDiscount(payment, discount);

            Assert.AreEqual(90, result.totalWithDiscount);
            Assert.AreEqual(1, result.discountId);
            Assert.AreEqual(10, result.discountAmount);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task AddToCart_WhenProductItemExists_IncreaseQuantity()
        {
     
            var userId = "testUserId";
            var productId = 1;

            var cartList = new List<Cart>
            {
                new Cart { CustomerID = userId, ProductID = 1, ProductQuantity = 1 },
                new Cart { CustomerID = "otherUserId", ProductID = 2, ProductQuantity = 1 },
            };

            var cartDbSetMock = new Mock<DbSet<Cart>>();
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartList.AsQueryable().Provider);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartList.AsQueryable().Expression);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartList.AsQueryable().ElementType);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartList.GetEnumerator());

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Cart).Returns(cartDbSetMock.Object);

            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, userId));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            var result = await controller.AddToCart(productId);

            var existingCartItem = cartList.SingleOrDefault(c => c.CustomerID == userId && c.ProductID == productId);
            Assert.IsNotNull(existingCartItem);
            Assert.AreEqual(2, existingCartItem.ProductQuantity);

            cartDbSetMock.Verify(d => d.Add(It.IsAny<Cart>()), Times.Never);
        }

        // written by : Aida Zametica
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddToCart_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
    
            var mockDiscountCodeVerifier = new Mock<IDiscountCodeVerifier>();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(a => a.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim)null);

            var controller = new DtoRequestsController(_dbContext, mockDiscountCodeVerifier.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
                }
            };

            await controller.AddToCart(1); 
        }

        // written by: Aida Zametica
        [TestMethod]
        public async Task AddToCart_WhenProductItemNotFound_CreateCart()
        {

            var userId = "testUserId";
            var productId = 1;

            var cartList = new List<Cart>
            {
                new Cart { CustomerID = "otherUserId", ProductID = 2, ProductQuantity = 1 },
            };

            var cartDbSetMock = new Mock<DbSet<Cart>>();
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartList.AsQueryable().Provider);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartList.AsQueryable().Expression);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartList.AsQueryable().ElementType);
            cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartList.GetEnumerator());
    

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Cart).Returns(cartDbSetMock.Object);

            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, userId));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            dbContextMock.
             Setup(m => m.Add(It.IsAny<Cart>())).Callback<Cart>((entity) => cartList.Add(entity));

            var result = await controller.AddToCart(productId);
            Assert.AreEqual(2, cartList.Count);

            var newCartItem = cartList.SingleOrDefault(c => c.CustomerID == userId && c.ProductID == productId);
            Assert.AreEqual(1, newCartItem.ProductQuantity);
        }

        // written by : Aida Zametica
        [TestMethod]
        [DataRow(123, "Address123", 0)]
        [DataRow(null,"Address456", 1)]
        public async Task SavePaymentData_ShouldSavePaymentCorrectly(int? bankAccount, string deliveryAddress, PaymentType paymentType)
        {

            var mockDiscountCodeVerifier = new Mock<IDiscountCodeVerifier>();

            var dbContextMock = new Mock<ApplicationDbContext>();

            var controller = new DtoRequestsController(dbContextMock.Object, mockDiscountCodeVerifier.Object);

            var payment = new Payment
            {
                BankAccount = bankAccount,
                DeliveryAddress = deliveryAddress,
                PaymentType =paymentType,
            };

            double totalWithDiscount = 50.0;
            int? discountId = 1;


            var result = await controller.SavePaymentData(payment, totalWithDiscount, discountId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.BankAccount,bankAccount);
            Assert.AreEqual(result.DeliveryAddress, deliveryAddress);
            Assert.AreEqual(result.PaymentType, paymentType);
            Assert.AreEqual(result.PayedAmount, totalWithDiscount);
            Assert.AreEqual(result.DiscountID, discountId);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task SaveOrderData_ShouldSaveOrderCorrectly()
        {

            var mockDiscountCodeVerifier = new Mock<IDiscountCodeVerifier>();

            var dbContextMock = new Mock<ApplicationDbContext>();

            var controller = new DtoRequestsController(dbContextMock.Object, mockDiscountCodeVerifier.Object);

            double totalWithDiscount = 50.0;
            string userId = "testUser";
            Payment paymentForOrder = new Payment { PaymentID = 1 };

            var order = new Order
            {
                DeliveryDate=new DateTime(),
                personalMessage = "Test message"
            };

            var result = await controller.SaveOrderData(order, userId, paymentForOrder,totalWithDiscount);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.TotalAmountToPay, totalWithDiscount);
            Assert.AreEqual(result.CustomerID, userId);
            Assert.AreEqual(result.PaymentID,paymentForOrder.PaymentID);
            Assert.IsFalse(result.IsOrderSent);
            Assert.IsNull(result.Rating);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void ThankYou_ReturnsViewWithOrderType()
        {
  
            var orderType = "TestOrderType";
            var dbContextMock = new Mock<ApplicationDbContext>();
            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            var result = controller.ThankYou(orderType) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(orderType, result.ViewData["OrderType"]);
        }

        /*
        // written by : Aida Zametica
        [TestMethod]
        public async Task OrderCreate_WithValidData_ReturnsRedirectToThankYou()
        {
            var userId = "testUserId";

            var usersDbSetMock = new Mock<DbSet<ApplicationUser>>();
            usersDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(userId.AsQueryable().Provider);
            usersDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(userId.AsQueryable().Expression);
            usersDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(userId.AsQueryable().ElementType);
            usersDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(userId.GetEnumerator());
 
            dbContextMock.Setup(c => c.Users).Returns(usersDbSetMock.Object);

            var dbContextMock = new Mock<ApplicationDbContext>();
            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, userId));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            var order = new Order {
                OrderID = 1,
                DeliveryDate = new DateTime(),
                personalMessage = "Test message"
            };

            var discount = new Discount { 
                DiscountID = 1,
                DiscountCode = "ValidDiscountCode" 
            };


            var payment = new Payment
            {
                PaymentID = 1,
    
            };

            var result = await controller.OrderCreate(order, payment, discount) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ThankYou", result.ActionName);
            Assert.AreEqual("order", result.RouteValues["orderType"]);

        }
        */


        [TestCleanup]
        public async Task TestCleanup()
        {
            await _dbContext.Database.EnsureDeletedAsync();
 
        }


    }
}