using Scaffoldong.Models;

namespace Scaffoldong.ViewModel
{
    public class EquipmentViewModel
    {
        public IEnumerable<Equipment> Equipment { get; set; }
        public IEnumerable<Equipment_Details> Equipment_Details { get; set; }
        public string EName { get; set; }
        public string EName_old { get; set; }
        public string EModel { get; set; }
        public string Emodel_old { get; set; }
        public string EQuantity { get; set; }
        public string EQuantityYes { get; set; }
        public string EQuantityNo { get; set; }
        public string EBorrowing_Quantity { get; set; }
        public string ERemaining_Quantity { get; set; }

        public string EMissing_quantity { get; set; }
        public List<string> EId { get; set; }
        public List<string> IsBorrow { get; set; }
        public List<string> ECurrent_Location_update { get; set; }
        public string EQuantity_Unit { get; set; }
        public string ESource { get; set; }
        public string ESource_old { get; set; }
        
        public string EImage_old { get; set; }
        public IFormFile FileInput { get; set; }
        public string ECurrent_Location { get; set; }
        public string Is_Consumable { get; set; }
    }
}
