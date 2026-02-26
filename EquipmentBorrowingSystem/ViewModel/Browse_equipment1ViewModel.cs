using EquipmentBorrowingSystem.Models;

namespace EquipmentBorrowingSystem.ViewModel
{
    public class Browse_equipment1ViewModel
    {
        public IEnumerable<Equipment> Equipment { get; set; }
      
        public string keyword { get; set; }
        public string select { get; set; }
    }
}
