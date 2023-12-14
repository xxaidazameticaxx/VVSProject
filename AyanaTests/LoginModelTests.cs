
using Ayana.Areas.Identity.Pages.Account;
using Ayana.Data;
using Ayana.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel.DataAnnotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AyanaTests
{
    [TestClass]
    public class LoginModelTests
    {
        private LoginModel _loginModel;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<LoginModel>> _mockLogger;

        [TestInitialize]
        public void Initialize()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);


            _mockLogger = new Mock<ILogger<LoginModel>>();

            _loginModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object, _mockUserManager.Object);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void InputModel_Email_ShouldBeRequired()
        {
            // Arrange
            var inputModel = new LoginModel.InputModel { Email = null, Password = "password", RememberMe = false };
            var validationContext = new ValidationContext(inputModel, null, null);

            // Act
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(inputModel, validationContext, validationResults, true);

            // Assert
            var emailValidationResult = validationResults.FirstOrDefault(v => v.MemberNames.Contains("Email"));
            Assert.IsNotNull(emailValidationResult);
            Assert.AreEqual("The Email field is required.", emailValidationResult.ErrorMessage);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task OnGetAsync_ClearsExternalCookie()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationManager = new Mock<IAuthenticationService>();
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
            var mockUrlHelper = new Mock<IUrlHelper>();  

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationManager.Object);

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            mockUrlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(mockUrlHelper.Object);  

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url"); 

            mockAuthenticationManager.Setup(x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _loginModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            await _loginModel.OnGetAsync();

            // Assert
            mockAuthenticationManager.Verify(
                x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()),
                Times.Once);

            // Verify that returnUrl is set to the expected value
            Assert.AreEqual("mocked-url", _loginModel.ReturnUrl);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task OnPostAsync_ValidModelState_Succeeds_ReturnsLocalRedirect()
        {
            // Arrange
            _loginModel.Input = new LoginModel.InputModel
            {
                Email = "valid-email@example.com",
                Password = "valid-password",
                RememberMe = false
            };

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationManager = new Mock<IAuthenticationService>();
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationManager.Object);

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            mockUrlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(mockUrlHelper.Object);

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url");

            mockAuthenticationManager.Setup(x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _loginModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            };


            // Arrange
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);  

            // Act
            var result = await _loginModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.LocalRedirectResult));
            var redirectResult = (LocalRedirectResult)result;
            Assert.AreEqual("mocked-url", redirectResult.Url);
        }

        // written by : Aida Zametica 
        [TestMethod]
        public async Task OnPostAsync_RequiresTwoFactor_RedirectsToLoginWith2fa()
        {
            // Arrange
            _loginModel.Input = new LoginModel.InputModel
            {
                Email = "valid-email@example.com",
                Password = "valid-password",
                RememberMe = false
            };

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationManager = new Mock<IAuthenticationService>();
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationManager.Object);

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            mockUrlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(mockUrlHelper.Object);

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url");

            mockAuthenticationManager.Setup(x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _loginModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Arrange
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.TwoFactorRequired);

            // Act
            var result = await _loginModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));

            var redirectResult = (RedirectToPageResult)result;
            Assert.AreEqual("./LoginWith2fa", redirectResult.PageName);

        }

        // written by : Aida Zametica and Almedin Pašalić
        [TestMethod]
        public async Task OnPostAsync_IsLockedOut_RedirectsToLoginWithLockout()
        {
            // Arrange
            _loginModel.Input = new LoginModel.InputModel
            {
                Email = "valid-email@example.com",
                Password = "valid-password",
                RememberMe = false
            };

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationManager = new Mock<IAuthenticationService>();
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationManager.Object);

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            mockUrlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(mockUrlHelper.Object);

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url");

            mockAuthenticationManager.Setup(x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _loginModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Arrange
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut);

            // Act
            var result = await _loginModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));

            var redirectResult = (RedirectToPageResult)result;
            Assert.AreEqual("./Lockout", redirectResult.PageName);

        }

        // written by : Aida Zametica and Almedin Pašalić
        [TestMethod]
        public async Task OnPostAsync_Failed_ReturnsToLoginPage()
        {

            // Arrange
            _loginModel.Input = new LoginModel.InputModel
            {
                Email = "valid-email@example.com",
                Password = "valid-password",
                RememberMe = false
            };

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationManager = new Mock<IAuthenticationService>();
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationManager.Object);

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            mockUrlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(mockUrlHelper.Object);

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url");

            mockAuthenticationManager.Setup(x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _loginModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Arrange
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _loginModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(PageResult));

        }

        // written by : Almedin Pašalić
        [TestMethod]
        public async Task OnPostAsync_WhenModelIsNotValid()
        {
            _loginModel.Input = new LoginModel.InputModel
            {
                Email = "valid-email@example.com",
                Password = "valid-password",
                RememberMe = false
            };

            var mockHttpContext = new Mock<HttpContext>();
            var mockAuthenticationManager = new Mock<IAuthenticationService>();
            var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthenticationManager.Object);

            mockHttpContext.Setup(c => c.RequestServices.GetService(typeof(IUrlHelperFactory)))
                .Returns(mockUrlHelperFactory.Object);

            mockUrlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(mockUrlHelper.Object);

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url");

            mockAuthenticationManager.Setup(x => x.SignOutAsync(mockHttpContext.Object, IdentityConstants.ExternalScheme, It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _loginModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Arrange
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            _loginModel.ModelState.AddModelError("Email", "Invalid email");

            // Act
            var result = await _loginModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(PageResult));



        }




    }
}
