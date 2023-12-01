using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Moq;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace AyanaTests
{
   
    [TestClass]
    public class DtoRequestsControllerTests
    {
     
        // written by : Aida Zametica
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task RemoveItem_WhenUserIdIsNull_ThrowsArgumentNullException()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);

            var mockDiscountCodeVerifier = new Mock<IDiscountCodeVerifier>();

            // Mock HttpContextAccessor to simulate a null User
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(a => a.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim)null);

            var controller = new DtoRequestsController(dbContext, mockDiscountCodeVerifier.Object)
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

        [TestMethod]
        public async Task ApplyDiscount_ValidDiscountCodeAndNotExpired_ShouldApplyDiscountCorrectly()
        {
            // Arrange
            var payment = new Payment
            {
                PayedAmount = 100 // Set a value based on your test scenario
            };

            var discount = new Discount
            {
                DiscountCode = "ValidDiscountCode" // Set a valid discount code based on your test scenario
            };

            // Mock the IDiscountCodeVerifier
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

            // Act
            var result = await controller.CalculateDiscount(payment, discount);

            // Assert
            Assert.AreEqual(90, result.totalWithDiscount); // Adjust based on your expected result
            Assert.AreEqual(1, result.discountId); // Adjust based on your expected result
            Assert.AreEqual(10, result.discountAmount); // Adjust based on your expected result
        }


    }
}
