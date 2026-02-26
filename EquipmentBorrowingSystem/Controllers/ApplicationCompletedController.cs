using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipmentBorrowingSystem.Data;
using EquipmentBorrowingSystem.Migrations;
using EquipmentBorrowingSystem.Models;
using EquipmentBorrowingSystem.EMail;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EquipmentBorrowingSystem.Controllers
{
    public class ApplicationCompletedController : Controller
    {
        private readonly EmployeeContext _context;
        private readonly IEmailSender emailSender;

        public ApplicationCompletedController(EmployeeContext context, IEmailSender emailSender)
        {
            _context = context;
            this.emailSender = emailSender;

        }
        public IActionResult Index()
        {
            return View();
        }



        //當這個動作ApplicationCompleted被呼叫時，直接去判斷目前借用紀錄中是否有設備逾期未還，如果有就發送email通知
        public IActionResult ApplicationCompleted()
        {
            string fUserId = User.Identity.Name;

            //查詢全部，申請成功的設備借用紀錄
            var application_completed_All = _context.Application_Completed
              .Where(m => m.Status != "Returned")
              .OrderByDescending(m => m.Borrow_Time)
              .ToList();


            //判斷設備歸還日期是否逾期
            foreach (var item in application_completed_All)
            {
                if (item.Return_Time < DateTime.Now)
                {
                    if (item.IsSendEmail == "False")
                    {
                        item.IsSendEmail = "True";
                        item.Status = "Overdue";
                        SendEmail_OverdueNotificationController sendemail_OverdueNotification = new SendEmail_OverdueNotificationController(emailSender, _context);
                        sendemail_OverdueNotification.SendEmail_OverdueNotification(item.fEmail, item.fOrderGuid);
                    }

                }
                _context.SaveChanges();
            }

            //將全部借用紀錄特過viewmodel傳遞至view
            var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
            {
                Application_Completed = application_completed_All
            };
            return View(ViewModel);
        }

        //再次寄送逾期通知
        public IActionResult Sendemail_OverdueNotification_Again(string fOrderGuid)
        {
            string fUserId = User.Identity.Name;

            var fUserId1 = _context.tMember
.Where(m => m.fUserId == "Kao77")
.FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代

            var application_completed_All = _context.Application_Completed
              .Where(m => m.fUserId == fUserId1.fUserId && m.fOrderGuid == fOrderGuid).ToList();

            foreach (var item in application_completed_All)
            {

                SendEmail_OverdueNotificationController sendemail_OverdueNotification = new SendEmail_OverdueNotificationController(emailSender, _context);
                sendemail_OverdueNotification.SendEmail_OverdueNotification(item.fEmail, item.fOrderGuid);

                _context.SaveChanges();
            }
            return RedirectToAction("ApplicationCompleted");
        }






        [HttpPost]
        //已接受此申請，傳送申請編號、移交人姓名、借用人抵押證件//已接受此申請，傳送申請編號、移交人姓名、借用人抵押證件//已接受此申請，傳送申請編號、移交人姓名、借用人抵押證件
        public ActionResult ApplicationCompleted(string fOrderGuid, string Credentials_Mortgage, string Equipment_Handover_Person, string IntputType, DateTime Date1, DateTime Date2, string Borrow_Name)
        {
            //判斷是否為接受設備申請
            if (fOrderGuid != null && Credentials_Mortgage != null && Equipment_Handover_Person != null)
            {
                string fUserId = User.Identity.Name;

                var fUserId1 = _context.tMember
                    .Where(m => m.fUserId == "Kao77")
                    .FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代


                var select_application = _context.Application
                    .Where(m => m.fOrderGuid == fOrderGuid).ToList();

                var select_application_details = _context.Application_Details.Where(m => m.fOrderGuid == fOrderGuid && m.Is_Consumable == "False").ToList();
                var select_application_details_Consumable = _context.Application_Details.Where(m => m.fOrderGuid == fOrderGuid && m.Is_Consumable == "True").ToList();
                var Borrow_Time = (select_application.FirstOrDefault().Borrow_Time).ToString("yyyy/M/d");

                //判斷是否為返回建(因Equipment_Recive_Person.cshtml案返回鍵時會傳送fOrderGuid值)，如(Credentials_Mortgage == null && Equipment_Handover_Person == null)代表是返回建
                if (Credentials_Mortgage == null && Equipment_Handover_Person == null)
                {
                    return RedirectToAction("ApplicationCompleted");
                }



                if (select_application.FirstOrDefault().Borrow_Time < DateTime.Now) //判斷借用時間是否小於現在時間
                {


                    foreach (var item in select_application_details)
                    {
                        //找出成功申請的設備名稱及編號，將IsBorrow和IsAddEquipment判斷是否借用改為True 代表為已借用
                        var select_Equipment_Details = _context.Equipment_Details.Where(m => m.EName == item.EName && m.EId == item.EId && m.Emodel == item.Emodel && m.ESource == item.ESource).ToList();
                        select_Equipment_Details.FirstOrDefault().IsAddEquipment = "True";
                        select_Equipment_Details.FirstOrDefault().IsBorrow = "True";
                        select_Equipment_Details.FirstOrDefault().ECurrent_Location = "借用人：" + select_application.FirstOrDefault().Name + "；借用日：" + Borrow_Time;
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
                    }



                    foreach (var item in select_application_details_Consumable)
                    {
                        var select_Consumable = _context.Equipment.Where(m => m.EName == item.EName && m.Emodel == item.Emodel && m.ESource == item.ESource && m.Is_Consumable == "True").ToList();

                        select_Consumable.FirstOrDefault().EBorrowing_Quantity = (int.Parse(select_Consumable.FirstOrDefault().EBorrowing_Quantity) + int.Parse(item.Consumable_Borrowing_Times)).ToString();
                        if (select_Consumable.FirstOrDefault().ERemaining_Quantity != "∞")
                        {
                            select_Consumable.FirstOrDefault().ERemaining_Quantity = (int.Parse(select_Consumable.FirstOrDefault().ERemaining_Quantity) - int.Parse(item.Consumable_Borrowing_Times)).ToString();
                        }

                    }



                }
                if (select_application != null && select_application.FirstOrDefault().Status == "Applying") //判斷申請清單中是否有這筆資料以及判斷是否為審核中
                {
                    //將申請狀態改為True，代表審核通過
                    select_application.FirstOrDefault().Status = "True";


                    Models.Application_Completed application_completed = new Models.Application_Completed();
                    application_completed.fOrderGuid = fOrderGuid;
                    application_completed.fUserId = fUserId1.fUserId;
                    application_completed.Date_Of_Application = select_application.FirstOrDefault().Date_Of_Application;
                    application_completed.Name = select_application.FirstOrDefault().Name;
                    application_completed.Borrow_Time = select_application.FirstOrDefault().Borrow_Time;
                    application_completed.Return_Time = select_application.FirstOrDefault().Return_Time;
                    application_completed.Mobile = select_application.FirstOrDefault().Mobile;
                    application_completed.fEmail = select_application.FirstOrDefault().fEmail;
                    application_completed.Illustrate = select_application.FirstOrDefault().Illustrate;
                    if (select_application.FirstOrDefault().Borrow_Time <= DateTime.Now)
                    {
                        application_completed.Status = "Borrowing";
                    }
                    else if (select_application.FirstOrDefault().Borrow_Time > DateTime.Now)
                    {
                        application_completed.Status = "Not_borrowed_yet";
                    }

                    application_completed.Credentials_Mortgage = Credentials_Mortgage;
                    application_completed.Equipment_Handover_Person = Equipment_Handover_Person;
                    application_completed.Equipment_Recive_Person = " ";
                    application_completed.IsSendEmail = "False";
                    _context.Application_Completed.Add(application_completed);

                }


                _context.SaveChanges();
                return RedirectToAction("ApplicationCompleted");
            }
            //判斷是否為查詢目前借用中的紀錄
            if (IntputType != null && Date1 != null && Date2 != null || Borrow_Name != null)
            {
                string fUserId = User.Identity.Name;

                var fUserId1 = _context.tMember
    .Where(m => m.fUserId == "Kao77")
    .FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代

                // 在後端代碼中處理日期
                DateTime date2 = Convert.ToDateTime(Request.Form["Date2"]);
                date2 = date2.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                if (Borrow_Name == null)
                {
                    if (IntputType == "ApplicationDate")
                    {
                        var select_application_completed = _context.Application_Completed
    .Where(m => m.Status == "Returned" && m.Date_Of_Application >= Date1 && m.Date_Of_Application <= date2)
    .OrderByDescending(m => m.Borrow_Time)
    .ToList();
                        string Date1String = Date1.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                        string Date2String = Date2.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                        var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
                        {
                            Application_Completed = select_application_completed,
                            IntputType = IntputType,
                            Date1 = Date1String,
                            Date2 = Date2String
                        };

                        return View(ViewModel);
                    }
                    else if (IntputType == "BorrowDate")
                    {
                        var select_application_completed = _context.Application_Completed
    .Where(m => m.Status == "Returned" && m.Borrow_Time >= Date1 && m.Borrow_Time <= date2)
    .OrderByDescending(m => m.Borrow_Time)
    .ToList();
                        string Date1String = Date1.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                        string Date2String = Date2.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                        var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
                        {
                            Application_Completed = select_application_completed,
                            IntputType = IntputType,
                            Date1 = Date1String,
                            Date2 = Date2String
                        };

                        return View(ViewModel);
                    }
                    else if (IntputType == "ReturnDate")
                    {
                        var select_application_completed = _context.Application_Completed
    .Where(m => m.Status == "Returned" && m.Return_Time >= Date1 && m.Return_Time <= date2)
    .OrderByDescending(m => m.Borrow_Time)
    .ToList();
                        string Date1String = Date1.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                        string Date2String = Date2.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                        var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
                        {
                            Application_Completed = select_application_completed,
                            IntputType = IntputType,
                            Date1 = Date1String,
                            Date2 = Date2String
                        };

                        return View(ViewModel);
                    }

                }
                else
                {
                    var select_application_completed = _context.Application_Completed
                    .Where(m => m.Name.Contains(Borrow_Name) && m.Status != "Returned" )
                    .OrderByDescending(m => m.Borrow_Time)
                    .ToList();
                    var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
                    {
                        Application_Completed = select_application_completed,
                        IntputType = IntputType,
                        Borrow_Name = Borrow_Name,
                    };

                    return View(ViewModel);
                }

            }
            return View();
        }



        //查詢已申請成功的借用設備詳細資訊//查詢已申請成功的借用設備詳細資訊//查詢已申請成功的借用設備詳細資訊
        public ActionResult Select_ApplicationCompleted_List(string fOrderGuid)
        {
            //根據fOrderGuid找出和訂單主檔關聯的訂單明細，並指定給orderDetails
            var select_application_list = _context.Application_Details
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();
            var select_application = _context.Application
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();

            var viewModel = new SelectApplicationViewModel
            {
                ApplicationDetails = select_application_list,
                Application = select_application,
                Name = select_application.FirstOrDefault().Name,
                EName = select_application_list.FirstOrDefault().EName,
                EId = select_application_list.FirstOrDefault().EId,
                Borrow_Time = select_application.FirstOrDefault().Borrow_Time,
                Return_Time = select_application.FirstOrDefault().Return_Time
            };       //目前訂單明細的OrderDetail.cshtml檢視使用orderDetails模型
            return View(viewModel);
        }




    }
}

