using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowingSystem.ViewModel
{
    public class BrowseEquipmentViewModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
