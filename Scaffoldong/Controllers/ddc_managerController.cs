using Microsoft.AspNetCore.Mvc;

namespace Scaffoldong.Controllers
{

    public class ddc_managerController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to ApplicationCompleted action in ApplicationCompleted controller
            return RedirectToAction("ApplicationCompleted", "ApplicationCompleted");
        }

        // Add this method to handle requests to http://localhost:5176/ddc_manager
        [HttpGet("ddc_manager")]
        public IActionResult RedirectToApplicationCompleted()
        {
            // Redirect to ApplicationCompleted action in ApplicationCompleted controller
            return RedirectToAction("ApplicationCompleted", "ApplicationCompleted");
        }
    }
}
