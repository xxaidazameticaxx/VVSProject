using System.Xml.Linq;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.AspNetCore.Mvc;
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

        public static IEnumerable<object[]> GetXmlTestData()
        {
            // Read data from the CSV file
            var doc = XDocument.Load("../../../TestData/ReportTypes.xml");

            foreach (var type in doc.Descendants("ReportType"))
            {
                int reportId = int.Parse(type.Element("id").Value);
                string reportName = type.Element("Name").Value;

                yield return new object[] { reportId, reportName };
            }
        }

        // written by : Lejla Heleg
        [TestMethod]
        [DynamicData(nameof(GetXmlTestData), DynamicDataSourceType.Method)]
        public void CreateReport_DifferentReportTypes_ReturnsFileResult(int id, string name)
        {
            var result = controller.CreateReport(name);

            Assert.IsInstanceOfType(result, typeof(FileResult));
        }
    }
}
