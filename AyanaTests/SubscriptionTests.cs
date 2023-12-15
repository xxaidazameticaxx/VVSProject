using System.Security.Claims;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class SubscriptionTests
    {
        private Mock<IDiscountCodeVerifier> discountCodeVerifierMock;
        private Mock<ClaimsPrincipal> userMock;
        private Mock<ApplicationDbContext> dbContextMock;
        private Subscription testSubscription = new Subscription
        {
            SubscriptionID = 1,
            Name = "Test Subscription",
            Price = 20.0,
            DeliveryDate = DateTime.Now.AddDays(5),
            CustomerID = "userId",
            PaymentID = 1,
            personalMessage = "Thank you for subscribing!",
            ApplicationUser = new ApplicationUser(),
            Payment = new Payment()
        };

        [TestInitialize]
        public void TestInitialize()
        {
            discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();

            dbContextMock = new Mock<ApplicationDbContext>();

            userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));
        }

        [TestMethod]
        public async Task PrepareSubscriptionOrder_ReturnsViewResultWithCorrectData()
        {

            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            var result = await controller.SubscriptionOrder("Test Subscription", 20.0);

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var subscriptionName = viewResult.ViewData["SubscriptionName"] as string;
            var subscriptionPrice = (double)viewResult.ViewData["SubscriptionPrice"];

            Assert.AreEqual("Test Subscription", subscriptionName, "SubscriptionName should match.");
            Assert.AreEqual(20.0, subscriptionPrice, "SubscriptionPrice should match.");
        }

        [TestMethod]
        public async Task SubscriptionCreate_ValidMonthSubscription_ReturnsRedirectToThankYou()
        {
            var controller = new Mock<DtoRequestsController>(dbContextMock.Object, discountCodeVerifierMock.Object)
            {
                CallBase = true
            };

            controller.Object.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            var result = await controller.Object.SubscriptionCreate(
                testSubscription,
                new Payment
                {
                    PaymentID = 1,
                    DeliveryAddress = "Travnicka cesta 29",
                    BankAccount = 123456789,
                    PaymentType = PaymentType.Cash,
                    PayedAmount = 20.0,
                    Discount = new Discount(),
                    DiscountID = null
                }
            );

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("ThankYou", redirectToActionResult.ActionName, "ActionName should be 'Thank you'.");
        }

        [TestMethod]
        public async Task SubscriptionCreate_ValidThreeMonthSubscription_ReturnsRedirectToThankYou()
        {
            var controller = new Mock<DtoRequestsController>(dbContextMock.Object, discountCodeVerifierMock.Object)
            {
                CallBase = true
            };

            controller.Object.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            testSubscription.Name = "Three month Package";

            var result = await controller.Object.SubscriptionCreate(
                testSubscription,
                new Payment
                {
                    PaymentID = 1,
                    DeliveryAddress = "Travnicka cesta 29",
                    BankAccount = 123456789,
                    PaymentType = PaymentType.Cash,
                    PayedAmount = 20.0,
                    Discount = new Discount(),
                    DiscountID = null
                }
            );

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("ThankYou", redirectToActionResult.ActionName, "ActionName should be 'Thank you'.");
        }

        [TestMethod]
        public async Task SubscriptionCreate_ValidSixMonthSubscription_ReturnsRedirectToThankYou()
        {
            var controller = new Mock<DtoRequestsController>(dbContextMock.Object, discountCodeVerifierMock.Object)
            {
                CallBase = true
            };

            controller.Object.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };


            testSubscription.Name = "Six month Package";

            var result = await controller.Object.SubscriptionCreate(
                testSubscription,
                new Payment
                {
                    PaymentID = 1,
                    DeliveryAddress = "Travnicka cesta 29",
                    BankAccount = 123456789,
                    PaymentType = PaymentType.Cash,
                    PayedAmount = 20.0,
                    Discount = new Discount(),
                    DiscountID = null
                }
            );

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("ThankYou", redirectToActionResult.ActionName, "ActionName should be 'Thank you'.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SubscriptionCreate_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(a => a.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim)null);

            var controller = new DtoRequestsController(dbContextMock.Object, discountCodeVerifierMock.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            await controller.SubscriptionCreate(new Subscription(), new Payment());
        }
    }
}
