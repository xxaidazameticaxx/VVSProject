using Ayana.MailgunService;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AyanaTests
{
    [TestClass]
    public class EmailServiceTests
    {
        // written by: Ilhan Hasičić
        [TestMethod]
        public void SendVerificationCode_ShouldCallSendEmailToCustomer()
        {
            var mailgunServiceMock = new Mock<IMailgunService>();
            var emailService = new EmailService(mailgunServiceMock.Object);

            emailService.SendVerificationCode("test@example.com");

            mailgunServiceMock.Verify(
                x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        // written by: Ilhan Hasičić
        [TestMethod]
        public void GetVerificationCode_ShouldReturnNullWhenCacheIsEmpty()
        {
            var emailService = new EmailService(Mock.Of<IMailgunService>());

            var result = emailService.GetVerificationCode();

            Assert.IsNull(result);
        }

        // written by: Ilhan Hasičić
        [TestMethod]
        public void VerifyCode_ShouldReturnTrueForValidCode()
        {
            var emailService = new EmailService(Mock.Of<IMailgunService>());
            var code = emailService.GenerateCode();
            emailService.SaveVerificationCode(code, TimeSpan.FromMinutes(10));

            var result = emailService.VerifyCode("test@example.com", code);

            Assert.IsTrue(result);
        }

        // written by: Ilhan Hasičić
        [TestMethod]
        public void VerifyCode_ShouldReturnFalseForInvalidCode()
        {
            var emailService = new EmailService(Mock.Of<IMailgunService>());
            emailService.SaveVerificationCode("validCode", TimeSpan.FromMinutes(10));

            var result = emailService.VerifyCode("test@example.com", "invalidCode");

            Assert.IsFalse(result);
        }

        // written by: Ilhan Hasičić
        [TestMethod]
        public void GenerateCode_ShouldReturn4DigitCode()
        {
            var emailService = new EmailService(Mock.Of<IMailgunService>());

            var code = emailService.GenerateCode();

            Assert.IsTrue(int.TryParse(code, out int codeValue) && codeValue >= 1000 && codeValue <= 9999);
        }


    }

}

