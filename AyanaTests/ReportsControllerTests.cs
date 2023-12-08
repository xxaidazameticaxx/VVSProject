using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class ReportsControllerTests
    {
        private ReportsController controller;
        private Mock<ApplicationDbContext> contextMock;

        [TestInitialize]
        public void TestInitialize()
        {
            contextMock = new Mock<ApplicationDbContext>();
            var reportFactoryMock = new Mock<IReportFactory>();

            controller = new ReportsController(contextMock.Object, reportFactoryMock.Object);

            var fakeReport = new Mock<IReport>();
            fakeReport.Setup(report => report.GenerateReport()).Returns(new byte[] { 1, 2, 3 }); // Replace with actual report data
            reportFactoryMock.Setup(factory => factory.CreateReport(It.IsAny<string>())).Returns(fakeReport.Object);

        }

        // written by : Lejla Heleg
        [TestMethod]
        public void ReportConstructor_WithParameters_ObjectInstanceMade()
        {
            var report = new Report
            {
                ReportID = 1,
                ReportType = ReportType.Monthly, 
                Date = DateTime.Now,
                EmployeeID = "employeeId",
                Employee = new ApplicationUser()
            };

            Assert.AreEqual(1, report.ReportID);
            Assert.AreEqual(ReportType.Monthly, report.ReportType);
            Assert.AreEqual(DateTime.Now.Date, report.Date.Date); 
            Assert.AreEqual("employeeId", report.EmployeeID);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void CreateReport_ReturnsFileResult()
        {
            var result = controller.CreateReport("weekly");

            Assert.IsNotNull(result);
        }
    }
}
