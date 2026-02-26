using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowingSystem.ViewModel
{
    public class Browse_Equipment_EId1ViewModel : Controller
    {
        public IEnumerable<Equipment_Details> Equipment_Details { get; set; }

        public string keyword { get; set; }
        public string select { get; set; }
    }
}
