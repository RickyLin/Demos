using Microsoft.AspNetCore.Mvc;

namespace CustomizedViewLocation.Reports.AdHocReports.EmployeeReport
{
    public class EmployeeReportController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Print() => View();
    }
}