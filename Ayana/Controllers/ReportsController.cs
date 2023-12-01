using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ayana.Data;
using Ayana.Models;
using Microsoft.AspNetCore.Authorization;
using Ayana.Paterni;

namespace Ayana.Controllers
{
    [Authorize(Roles = "Employee")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportFactory _reportFactory;

        public ReportsController(ApplicationDbContext context, IReportFactory reportFactory)
        {
            _context = context;
            _reportFactory = reportFactory;
        }

        public IActionResult CreateReport(string type)
        {
            // Use the factory to create an instance of IReport (specifically WeeklyReport)
            IReport weeklyReport = _reportFactory.CreateReport(type);

            // Generate the report
            byte[] reportData = weeklyReport.GenerateReport();

            // Return the report as a file
            return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", type + "_report.xlsx");
        }
    }
}
