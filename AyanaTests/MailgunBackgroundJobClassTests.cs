using Ayana.Data;
using Ayana.MailgunService;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class MailgunBackgroundJobClassTests
    {
        public static IEnumerable<object[]> GetCsvTestData()
        {
            // Read data from the CSV file
            var lines = File.ReadAllLines("../../../TestData/ApplicationUsers.csv");

            // Skip the header line and convert data to object arrays for parameterized testing
            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(',');
                yield return new object[] { int.Parse(values[0]), values[1], values[2], values[3], values[4] };
            }
        }

        // written by : Lejla Heleg
        [TestMethod]
        [DynamicData(nameof(GetCsvTestData), DynamicDataSourceType.Method)]
        public void CheckAndSendEmail_InactiveCustomersExist_SendsOneEmailEach(int id, string fullName, string email, string ayanaUsername, string password)
        {
            var emailServiceMock = new Mock<IEmailService>();
            var customerServiceMock = new Mock<ICustomerService>();

            var inactiveCustomers = new List<ApplicationUser>
            {
                new ApplicationUser { Email = email }
            };

            customerServiceMock.Setup(c => c.GetInactiveCustomers()).Returns(inactiveCustomers);

            var mailgunBackgroundJob = new MailgunBackgroundJob(emailServiceMock.Object, customerServiceMock.Object);

            mailgunBackgroundJob.Start();

            Thread.Sleep(TimeSpan.FromSeconds(5));

            mailgunBackgroundJob.Stop();

            emailServiceMock.Verify( e => e.SendEmailToCustomer(email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }

}

