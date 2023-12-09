using Ayana.Data;
using Ayana.MailgunService;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class MailgunBackgroundJobClassTests
    {
        // written by : Lejla Heleg
        [TestMethod]
        public void CheckAndSendEmail_InactiveCustomers_SendsOneEmailEach()
        {
            var emailServiceMock = new Mock<IEmailService>();
            var customerServiceMock = new Mock<ICustomerService>();

            var inactiveCustomers = new List<ApplicationUser>
            {
                new ApplicationUser { Email = "test1@example.com" },
                new ApplicationUser { Email = "test2@example.com" }
            };

            customerServiceMock.Setup(c => c.GetInactiveCustomers()).Returns(inactiveCustomers);

            var mailgunBackgroundJob = new MailgunBackgroundJob(emailServiceMock.Object, customerServiceMock.Object);

            mailgunBackgroundJob.Start();

            Thread.Sleep(TimeSpan.FromSeconds(5));

            mailgunBackgroundJob.Stop();

            emailServiceMock.Verify( e => e.SendEmailToCustomer("test1@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            emailServiceMock.Verify( e => e.SendEmailToCustomer("test2@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }

}

