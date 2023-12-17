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
using Microsoft.Extensions.Primitives;

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
        
        //written by Vedran Mujić
        
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

        
        //written by Vedran Mujić
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

        //written by Vedran Mujić

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

        //written by Vedran Mujić

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

        //written by Vedran Mujić

        [TestMethod]
        public async Task OnPostAsync_UserIsNull_ReturnsNotFound()
        {
            // Arrange
            var confirmationCode = "valid-code";
            string returnUrl = "/some-page";
            string email = "nonexistent@example.com";

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
       {
           { "Email", email }
       });
            _registerConfirmationModel.PageContext = new PageContext { HttpContext = httpContext };
            // Act
            var result = await _registerConfirmationModel.OnPostAsync(confirmationCode, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual($"Unable to load user with email '{email}'.", notFoundResult.Value);
        }

        //written by Vedran Mujić

        [TestMethod]
        public async Task OnPostAsync_ModelStateNotValid_ReturnsPage()
        {
            // Arrange
            var confirmationCode = "valid-code";
            string returnUrl = "/some-page";
            string email = "test@example.com";

            _registerConfirmationModel.Email = email;

            _registerConfirmationModel.ModelState.AddModelError("PropertyName", "Error message");

            // Act
            var result = await _registerConfirmationModel.OnPostAsync(confirmationCode, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
        }
        
        //written by Vedran Mujić

        [TestMethod]
        public async Task OnPostAsync_VerifyCodeFalse_DeletesUserAndReturnsRedirectToRegister()
        {
            // Arrange
            var confirmationCode = "invalid-code";
            string returnUrl = "/some-page";
            string email = "test@example.com";

            var user = new ApplicationUser { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockEmailService.Setup(x => x.VerifyCode(email, confirmationCode))
                .Returns(false);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
     {
         { "Email", email }
     });
            _registerConfirmationModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            var result = await _registerConfirmationModel.OnPostAsync(confirmationCode, returnUrl);

            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            var redirectResult = (RedirectToPageResult)result;
            Assert.AreEqual("/Account/Register", redirectResult.PageName);
        }

        //written by Vedran Mujić

        [TestMethod]
        public async Task OnPostAsync_VerifyCodeTrue_SignsInUserAndReturnsLocalRedirect()
        {
            // Arrange
            var confirmationCode = "valid-code";
            string returnUrl = "/some-page";
            string email = "test@example.com";

            var user = new ApplicationUser { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockEmailService.Setup(x => x.VerifyCode(email, confirmationCode))
                .Returns(true);

            var identityResult = IdentityResult.Success;
            _mockUserManager.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(identityResult);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
    {
        { "Email", email }
    });
            _registerConfirmationModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            var result = await _registerConfirmationModel.OnPostAsync(confirmationCode, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(LocalRedirectResult));
            var redirectResult = (LocalRedirectResult)result;
            Assert.AreEqual(returnUrl, redirectResult.Url);

            // Ensure user is signed in
            _mockSignInManager.Verify(x => x.SignInAsync(user, false, null), Times.Once);
        }

        //written by Vedran Mujić

        [TestMethod]
        public async Task OnPostAsync_VerifyCodeTrue_UpdateUserFails_AddsErrorsToModelState()
        {
            // Arrange
            var confirmationCode = "valid-code";
            string returnUrl = "/some-page";
            string email = "test@example.com";

            var user = new ApplicationUser { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockEmailService.Setup(x => x.VerifyCode(email, confirmationCode))
                .Returns(true);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error 1" },
                                                      new IdentityError { Description = "Error 2" });
            _mockUserManager.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(identityResult);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "Email", email }
            });
            _registerConfirmationModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            var result = await _registerConfirmationModel.OnPostAsync(confirmationCode, returnUrl);

            // Assert
            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.RedirectToPageResult)); // Corrected assertion type
            Assert.AreEqual(2, _registerConfirmationModel.ModelState.ErrorCount); // Check that errors are added

            // Optionally, check specific error messages
            Assert.IsTrue(_registerConfirmationModel.ModelState.ContainsKey(string.Empty));
            var errorMessages = _registerConfirmationModel.ModelState[string.Empty].Errors.Select(e => e.ErrorMessage).ToList();
            CollectionAssert.Contains(errorMessages, "Error 1");
            CollectionAssert.Contains(errorMessages, "Error 2");
        }






    }
}