using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EquipmentBorrowingSystem.Models
{
    public class Application_Details
    {
        [Key]
        public int fId { get; set; }

        [DisplayName("申請編號")]
        public string fOrderGuid { get; set; }
        [DisplayName("會員帳號")]
        public string fUserId { get; set; }
        [DisplayName("設備名稱")]
        public string EName { get; set; }

        [Display(Name = "設備型號")]
        public string Emodel { get; set; }
        [DisplayName("設備來源")]
        public string ESource { get; set; }
        [DisplayName("設備編號")]
        public string EId { get; set; }
        [DisplayName("是否申請")]
        public string fIsApplied { get; set; }

        [DisplayName("是否統計過")]
        public string IsCount_Borrowing_Times { get; set; }

        [Display(Name = "消耗品借用數量")]
        public string Consumable_Borrowing_Times { get; set; }

        [Display(Name = "是否為消耗品")]
        public string Is_Consumable { get; set; }

    }
}
