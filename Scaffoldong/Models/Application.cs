using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Scaffoldong.Models
{
    public class Application
    {
        [Key]
        public int fId { get; set; }

        [DisplayName("申請日期")]
        public DateTime Date_Of_Application { get; set; }

        [DisplayName("申請編號")]
        public string fOrderGuid { get; set; }

        [DisplayName("會員帳號")]
        public string fUserId { get; set; }

        [Display(Name = "借用人")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "借用時間")]
        public DateTime Borrow_Time { get; set; }

        [Display(Name = "歸還時間")]
        public DateTime Return_Time { get; set; }

        [Display(Name = "行動電話")]
        [Required]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "電話號碼格式錯誤，必須是09開頭，且為10位數")]
        public string Mobile { get; set; }

         [DisplayName("信箱")]
        [EmailAddress(ErrorMessage = "E-MAIL 格式有誤")]
        public string fEmail { get; set; }

        [Display(Name = "用途說明")]
        [Required]
        public string Illustrate { get; set; }

        [Display(Name = "審核狀態")]
        public string Status { get; set; }
    }
}
