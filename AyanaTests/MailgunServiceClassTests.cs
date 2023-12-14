using RestSharp;

namespace AyanaTests
{
    [TestClass]
    public class MailgunServiceClassTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void SendEmail_WithInvalidApiKey_Fails()
        {
            string apiKey = "invalid-api-key";
            string domain = "your-domain";
            MailgunServiceClass mailgunService = new MailgunServiceClass(apiKey, domain);

            RestResponse response = mailgunService.SendEmail("recipient@example.com", "Test Subject", "Test Message");

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccessful);
        }
    }
}
