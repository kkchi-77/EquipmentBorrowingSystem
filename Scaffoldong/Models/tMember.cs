using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Scaffoldong.Models
{


    public class tMember
    {
        [Key]
        public int fId { get; set; }
        [DisplayName("帳號")]
        [Required(ErrorMessage = "帳號不可空白")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "帳號必須包含至少一個字母和一個數字")]
        [StringLength(int.MaxValue, MinimumLength = 5, ErrorMessage = "帳號至少需5碼")]
        public string fUserId { get; set; }
        [DisplayName("密碼")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "密碼必須包含至少一個字母和一個數字")]
        [Required(ErrorMessage = "密碼不可空白")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "密碼必須位於8-16碼")]
        public string fPwd { get; set; }
        [DisplayName("姓名")]
        [Required(ErrorMessage = "姓名不可空白")]
        public string fName { get; set; }
        [DisplayName("信箱")]
        [EmailAddress(ErrorMessage = "E-MAIL 格式有誤")]
        public string fEmail { get; set; }
    }
}
