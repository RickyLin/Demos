using Microsoft.AspNetCore.Mvc;

namespace CustomizedViewLocation.Reports.AdHocReports.ProductionReport
{
    public class ProductionReportController : Controller
    {
        public IActionResult Index() => View();
    }
}