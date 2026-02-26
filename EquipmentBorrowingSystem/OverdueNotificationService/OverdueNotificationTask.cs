using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EquipmentBorrowingSystem.Controllers;
using EquipmentBorrowingSystem.Data;
using EquipmentBorrowingSystem.EMail;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EquipmentBorrowingSystem.OverdueNotificationService
{
    public class OverdueNotificationTask
    {
        private readonly EmployeeContext _context;
        private readonly IEmailSender _emailSender;

        public OverdueNotificationTask(EmployeeContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public void Run()
        {
            var application_completed_All = _context.Application_Completed
              .Where(m => m.Status != "Returned").ToList();

            //判斷設備歸還日期是否逾期
            foreach (var item in application_completed_All)
            {
                if (item.Return_Time < DateTime.Now)
                {
                    if (item.IsSendEmail == "False")
                    {
                        item.IsSendEmail = "True";
                        item.Status = "Overdue";
                        SendEmail_OverdueNotificationController sendemail_OverdueNotification = new SendEmail_OverdueNotificationController(_emailSender, _context);
                        sendemail_OverdueNotification.SendEmail_OverdueNotification(item.fEmail, item.fOrderGuid);
                    }

                }
                else if (item.Return_Time >= DateTime.Now)
                {
                    //if (item.Borrow_Time < DateTime.Now)
                    //{
                    //    item.Status = "Not_borrowed_yet";
                    //}else if (item.Borrow_Time >= DateTime.Now) 
                    //{
                    //    item.Status = "Borrowing";
                    //}
                }
                _context.SaveChanges();
            }
        }

        public void Run1()
        {

            var select_application_completed = _context.Application_Completed
           .Where(m => m.Status == "Not_borrowed_yet").ToList();
            if (select_application_completed.Count != 0)
            {
                foreach (var item in select_application_completed)
                {
                    if (item.Borrow_Time <= DateTime.Now)
                    {
                        string fOrderGuid = select_application_completed.FirstOrDefault().fOrderGuid;
                        DateTime Borrow_Time = select_application_completed.FirstOrDefault().Borrow_Time;
                        Run2(fOrderGuid, Borrow_Time);
                    }
                }

            }

        }
        public void Run2(string fOrderGuid, DateTime Borrow_Time)
        {
            var select_application_completed = _context.Application_Completed
           .Where(m => m.fOrderGuid == fOrderGuid).ToList();
            select_application_completed.FirstOrDefault().Status = "Borrowing";
            _context.SaveChanges();
            //找出申請的詳細資訊(包含設備名稱、設備編號、設備來源、申請編號)，並將成功申請的設備，借用數量+1、剩餘數量-1
            var select_application_details = _context.Application_Details.Where(m => m.fOrderGuid == fOrderGuid && m.Is_Consumable == "False").ToList();
            var select_application_details_Consumable = _context.Application_Details.Where(m => m.fOrderGuid == fOrderGuid && m.Is_Consumable == "True").ToList();
            foreach (var item in select_application_details)
            {
                //找出成功申請的設備名稱及編號，將IsBorrow和IsAddEquipment判斷是否借用改為True 代表為已借用
                var select_Equipment_Details = _context.Equipment_Details.Where(m => m.EName == item.EName && m.EId == item.EId && m.Emodel == item.Emodel && m.ESource == item.ESource).ToList();
                select_Equipment_Details.FirstOrDefault().IsAddEquipment = "True";
                select_Equipment_Details.FirstOrDefault().IsBorrow = "True";
                select_Equipment_Details.FirstOrDefault().ECurrent_Location = "借用人：" + select_application_completed.FirstOrDefault().Name + "；借用日：" + Borrow_Time;
                //找出成功申請的設備
                var select_Equipment = _context.Equipment.Where(m => m.EName == item.EName && m.ESource == item.ESource && m.Emodel == item.Emodel).ToList();
                //將設備借用數量+1
                var EBorrowing_Quantity = int.Parse(select_Equipment.FirstOrDefault().EBorrowing_Quantity);
                EBorrowing_Quantity++;
                select_Equipment.FirstOrDefault().EBorrowing_Quantity = EBorrowing_Quantity.ToString();
                //將設備剩餘數量-1
                var ERemaining_Quantity = int.Parse(select_Equipment.FirstOrDefault().ERemaining_Quantity);
                ERemaining_Quantity--;
                select_Equipment.FirstOrDefault().ERemaining_Quantity = ERemaining_Quantity.ToString();
                _context.SaveChanges();
            }
            foreach (var item in select_application_details_Consumable)
            {
                var select_Consumable = _context.Equipment.Where(m => m.EName == item.EName && m.Emodel == item.Emodel && m.ESource == item.ESource && m.Is_Consumable == "True").ToList();

                select_Consumable.FirstOrDefault().EBorrowing_Quantity = (int.Parse(select_Consumable.FirstOrDefault().EBorrowing_Quantity) + int.Parse(item.Consumable_Borrowing_Times)).ToString();
                if (select_Consumable.FirstOrDefault().ERemaining_Quantity != "∞")
                {
                    select_Consumable.FirstOrDefault().ERemaining_Quantity = (int.Parse(select_Consumable.FirstOrDefault().ERemaining_Quantity) - int.Parse(item.Consumable_Borrowing_Times)).ToString();
                }

                _context.SaveChanges();
            }
        }
    }
}