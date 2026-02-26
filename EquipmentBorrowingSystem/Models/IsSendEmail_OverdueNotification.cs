using System.ComponentModel;

namespace EquipmentBorrowingSystem.Models
{
    public class IsSendEmail_OverdueNotification
    {
        public int Id { get; set; }
        [DisplayName("申請編號")]
        public string Application_Number { get; set; }

        [DisplayName("是否寄過逾期通知")]
        public string IsSendEmail {  get; set; }

    }
}
