using System.ComponentModel.DataAnnotations;

namespace Scaffoldong.Models
{
    public class BorrowEquipment1
    {
        [Display(Name = "編號")]
        public int Id { get; set; }
        [Display(Name = "借用人")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "借用設備")]
        public string Equipment { get; set; }
        [Display(Name = "借用時間")]
        public string Borrow_Time { get; set; }
        [Display(Name = "歸還時間")]
        public string Return_Time { get; set; }
        [Display(Name = "行動電話")]
        [Required]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "電話號碼格式錯誤，必須是09開頭，且為10位數")]
        public string Mobile { get; set; }
        [Display(Name = "用途說明")]
        [Required]
        public string Illustrate { get; set; }
        [Display(Name = "審核狀態")]
        public string Status { get; set; }
    }
}
