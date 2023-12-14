using Ayana.Areas.Identity.Pages.Account;
using Ayana.Data;
using Ayana.MailgunService;
using Ayana.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AyanaTests
{
    [TestClass]
    public class RegisterModelTests
    {
        private RegisterModel _registerModel;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ILogger<RegisterModel>> _mockLogger;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<ApplicationDbContext> _mockDbContext;
        private Mock<IEmailService> _mockEmailService;

        [TestInitialize]
        public void Initialize()
        {
            // Konfiguracija UserManager-a
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);

            _mockLogger = new Mock<ILogger<RegisterModel>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockEmailService = new Mock<IEmailService>();
            _mockDbContext = new Mock<ApplicationDbContext>();

            // Konfiguracija RoleManager-a
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManager = new RoleManager<IdentityRole>(
                roleStore.Object,
                new IRoleValidator<IdentityRole>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null);

            _registerModel = new RegisterModel(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                roleManager,
                _mockLogger.Object,
                _mockEmailSender.Object,
                _mockDbContext.Object,
                _mockEmailService.Object);
        }

        // written by : Almedin Pašalić
        [TestMethod]
        public void InputModel_Email_ShouldBeRequired()
        {
            // Arrange
            var inputModel = new RegisterModel.InputModel
            {
                Email = null,
                Password = "valid-password",
                ConfirmPassword = "valid-password",
                FistName = "John",
                LastName = "Doe"
            };

            var validationContext = new ValidationContext(inputModel, null, null);

            // Act
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(inputModel, validationContext, validationResults, true);

            // Assert
            var emailValidationResult = validationResults.FirstOrDefault(v => v.MemberNames.Contains("Email"));
            Assert.IsNotNull(emailValidationResult);
            Assert.AreEqual("The Email field is required.", emailValidationResult.ErrorMessage);
        }

        // written by : Almedin Pašalić
        [TestMethod]
        public async Task OnGetAsync_SetsReturnUrlAndExternalLogins()
        {
            // Arrange
            var returnUrl = "/some-return-url";
            var externalAuthSchemes = new List<AuthenticationScheme>()
            {
                new AuthenticationScheme("Scheme1", "DisplayName1", typeof(IAuthenticationHandler)),
                new AuthenticationScheme("Scheme2", "DisplayName2", typeof(IAuthenticationHandler)),
            };

            _registerModel.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _registerModel.Url = new UrlHelper(new ActionContext
            {
                HttpContext = _registerModel.PageContext.HttpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            });

            _mockSignInManager.Setup(x => x.GetExternalAuthenticationSchemesAsync())
                .ReturnsAsync(externalAuthSchemes);

            // Act
            await _registerModel.OnGetAsync(returnUrl);

            // Assert
            Assert.AreEqual(returnUrl, _registerModel.ReturnUrl);

            Assert.IsNotNull(_registerModel.ExternalLogins);
            Assert.AreEqual(externalAuthSchemes.Count, _registerModel.ExternalLogins.Count);
        }

        // written by : Aida Zametica
        [TestMethod]
        public async Task OnPostAsync_ModelStateNotValid_ReturnsPageResult()
        {
            _registerModel.Input = new RegisterModel.InputModel
            {
                Email = "valid-email@example.com", 
                Password = "valid-password",
                ConfirmPassword = "valid-password",
                FistName = "John",
                LastName = "Doe"
            };

            var mockUrlHelper = new Mock<IUrlHelper>();

            mockUrlHelper.Setup(u => u.Content("~/"))
                .Returns("mocked-url");

            _registerModel.Url = mockUrlHelper.Object;

            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Simulating user creation failure" }));

            var result = await _registerModel.OnPostAsync();

            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.RazorPages.PageResult));
           
        }

       
    }
}
