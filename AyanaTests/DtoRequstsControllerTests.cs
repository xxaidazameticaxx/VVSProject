using Microsoft.EntityFrameworkCore;
using Moq;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;


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

            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.RedirectToActionResult));

            var redirectResult = (Microsoft.AspNetCore.Mvc.RedirectToActionResult)result;
            Assert.AreEqual("Cart", redirectResult.ActionName);
            Assert.AreEqual(0, redirectResult.RouteValues["discountAmount"]);
            Assert.AreEqual(1, redirectResult.RouteValues["discountType"]);
            Assert.AreEqual("", redirectResult.RouteValues["discountCode"]);

            var removedCartItem = cartList.SingleOrDefault(c => c.CustomerID == userId && c.ProductID == id);
            Assert.IsNull(removedCartItem, "The item should have been removed");

        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task RemoveItem_WhenProductQuantityEquaOne_DecreasesQuantity()
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
            // creates a mock object for the IDiscountCodeVerifier interface
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
        public async Task ApplyDiscount_ValidCodeAndNotExpiredPercentageOff_ShouldRedirectToCartWithDiscount()
        {

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            discountCodeVerifierMock.Setup(x => x.VerifyDiscountCode("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.VerifyExperationDate("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.GetDiscount("ValidDiscountCode")).Returns(new Discount
            {
                DiscountAmount = 10,
                DiscountType = DiscountType.PercentageOff
            });

            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.ApplyDiscount("ValidDiscountCode") as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("ValidDiscountCode", result.RouteValues["discountCode"]);
            Assert.AreEqual("Cart", result.ActionName);
        }


        // written by : Aida Zametica
        [TestMethod]
        public async Task ApplyDiscount_ValidCodeAndNotExpiredAmountOff_ShouldRedirectToCartWithDiscount()
        {

            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            discountCodeVerifierMock.Setup(x => x.VerifyDiscountCode("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.VerifyExperationDate("ValidDiscountCode")).Returns(true);
            discountCodeVerifierMock.Setup(x => x.GetDiscount("ValidDiscountCode")).Returns(new Discount
            {
                DiscountAmount = 10,
                DiscountType = DiscountType.AmountOff
            });

            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.ApplyDiscount("ValidDiscountCode") as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("ValidDiscountCode", result.RouteValues["discountCode"]);
            Assert.AreEqual("Cart", result.ActionName);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task ApplyDiscount_ValidCodeAndExpired_ShouldRedirectToCartWithDiscount()
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
        public async Task ApplyDiscount_InvalidCode_ShouldRedirectToCartWithDiscount()
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
        public async Task CalculateDiscount_ValidDiscountType2AndNotExpired_ShouldApplyDiscountCorrectly()
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
                DiscountType = DiscountType.PercentageOff
            });
            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.CalculateDiscount(payment, discount);

            Assert.AreEqual(90, result.totalWithDiscount);
            Assert.AreEqual(1, result.discountId);
            Assert.AreEqual(10, result.discountAmount);
        }

        //written by : Aida Zametica
        [TestMethod]
        public async Task CalculateDiscount_ValidDiscountType1AndNotExpired_ShouldApplyDiscountCorrectly()
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
                DiscountType = DiscountType.AmountOff
            });
            var controller = new DtoRequestsController(null, discountCodeVerifierMock.Object);

            var result = await controller.CalculateDiscount(payment, discount);

            Assert.AreEqual(90, result.totalWithDiscount);
            Assert.AreEqual(1, result.discountId);
            Assert.AreEqual(10, result.discountAmount);
        }



        [TestCleanup]
        public async Task TestCleanup()
        {
            await _dbContext.Database.EnsureDeletedAsync();
        }



    }
}