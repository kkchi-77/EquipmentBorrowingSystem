using System.ComponentModel.DataAnnotations;

namespace EquipmentBorrowingSystem.Models
{
    public class Equipment
    {
        [Display(Name = "設備")]
        public int Id { get; set; }
        [Display(Name = "設備名稱")]
        public string EName { get; set; }

        [Display(Name = "設備型號")]
        public string Emodel { get; set; }
        

        [Display(Name = "設備總數量")]
        public string EQuantity { get; set; }


        [Display(Name = "目前借用中數量")]
        public string EBorrowing_Quantity { get; set; }
        [Display(Name = "剩餘數量")]
        public string ERemaining_Quantity { get; set; }

        [Display(Name = "遺失數量")]
        public string EMissing_quantity { get; set; }

        [Display(Name = "數量單位")]
        public string EQuantity_Unit { get; set; }
        [Display(Name = "設備來源")]
        public string ESource { get; set; }

        [Display(Name = "設備圖片檔")]
        public string EImage { get; set; }
        [Display(Name = "是否為消耗品")]
        public string Is_Consumable { get; set; }
    }
}
