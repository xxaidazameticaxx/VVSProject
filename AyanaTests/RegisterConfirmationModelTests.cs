using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ayana.Areas.Identity.Pages.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ayana.Data;
using Ayana.MailgunService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AyanaTests
{
    [TestClass()]
    public class RegisterConfirmationModelTests
    {
        private RegisterConfirmationModel _registerConfirmationModel;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ApplicationDbContext> _mockDbContext;

        [TestInitialize]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);

            _mockEmailSender = new Mock<IEmailSender>();
            _mockEmailService = new Mock<IEmailService>();
            _mockDbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            _registerConfirmationModel = new RegisterConfirmationModel(
                _mockDbContext.Object,
                _mockEmailService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object,
                _mockEmailSender.Object);
        }

        [TestMethod]
        public async Task OnGetAsync_ValidEmail_ReturnsPageResult()
        {
            // Arrange
            var email = "test@example.com";

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(new ApplicationUser { Email = email });

            // Act
            var result = await _registerConfirmationModel.OnGetAsync(email);

            // Assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
        }
        [TestMethod]
        public async Task OnGetAsync_UserIsNull_ReturnsNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string returnUrl = "/some-page";

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _registerConfirmationModel.OnGetAsync(email, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual($"Unable to load user with email '{email}'.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task OnGetAsync_EmailIsNull_ReturnsRedirectToPageIndex()
        {
            // Arrange
            string email = null;
            string returnUrl = "/some-page";

            // Act
            var result = await _registerConfirmationModel.OnGetAsync(email, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            var redirectResult = (RedirectToPageResult)result;
            Assert.AreEqual("/Index", redirectResult.PageName);
        }

        [TestMethod]
        public async Task OnGetAsync_ValidEmail_SetsPropertiesCorrectly()
        {
            // Arrange
            var email = "test@example.com";
            var returnUrl = "/some-page";

            var user = new ApplicationUser { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _registerConfirmationModel.OnGetAsync(email, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.AreEqual(email, _registerConfirmationModel.Email);
            Assert.AreEqual(false, _registerConfirmationModel.DisplayConfirmAccountLink);
            Assert.IsNull(_registerConfirmationModel.EmailConfirmationUrl);

        }

      
        


    }
}