using System.ComponentModel.DataAnnotations;

namespace EquipmentBorrowingSystem.Models
{
    public class Count_Borrowing_Times
    {
        public int Id { get; set; }
        [Display(Name = "設備名稱")]
        public string EName { get; set; }

        [Display(Name = "設備型號")]
        public string Emodel { get; set; }


        [Display(Name = "設備總數量")]
        public string ECount_Borrowing_Times { get; set; }

    }
}
