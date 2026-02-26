using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Scaffoldong.Models
{
    public class SelectApplicationViewModel
    {
        public IEnumerable<Application> Application { get; set; }
        public IEnumerable<Application_Details> ApplicationDetails { get; set; }

        [Display(Name = "借用人")]
        [Required]
        public string Name { get; set; }

        [DisplayName("設備名稱")]
        public string EName { get; set; }
        [DisplayName("設備編號")]
        public string EId { get; set; }

        [Display(Name = "借用時間")]
        public DateTime Borrow_Time { get; set; }

        [Display(Name = "歸還時間")]
        public DateTime Return_Time { get; set; }
    }
}
