using System.ComponentModel.DataAnnotations;

namespace EquipmentBorrowingSystem.Models
{
    public class Equipment_Details
    {

        public int Id { get; set; }
        [Display(Name = "設備名稱")]
        public string EName { get; set; }


        [Display(Name = "設備型號")]
        public string Emodel { get; set; }

        [Display(Name = "設備編號")]
        public string EId { get; set; }

        [Display(Name = "設備來源")]
        public string ESource { get; set; }
        [Display(Name = "是否借用")]
        public string IsBorrow { get; set; }

        //[Display(Name = "設備數量")]
        //public string EQuantity { get; set; }

        //[Display(Name = "目前借用中數量")]
        //public string EBorrowing_Quantity { get; set; }
        //[Display(Name = "剩餘數量")]
        //public string ERemaining_Quantity { get; set; }
        //[Display(Name = "數量單位")]
        //public string EQuantity_Unit { get; set; }
        [Display(Name = "設備目前位置")]
        public string ECurrent_Location { get; set; }
        [Display(Name = "是否加入申請清單")]
        public string IsAddEquipment { get; set; }

    }
}
