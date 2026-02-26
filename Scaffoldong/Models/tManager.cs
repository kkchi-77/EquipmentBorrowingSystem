using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Scaffoldong.Models
{
    public class tManager
    {
        [Key]
        public int fId { get; set; }
        [DisplayName("管理者帳號")]
        public string fManagerID { get; set; }
        [DisplayName("密碼")]
        public string fManagerPwd { get; set; }
    }
}
