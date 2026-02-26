using Microsoft.AspNetCore.Mvc;

namespace Scaffoldong.ViewModel
{
    public class BrowseEquipmentViewModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
