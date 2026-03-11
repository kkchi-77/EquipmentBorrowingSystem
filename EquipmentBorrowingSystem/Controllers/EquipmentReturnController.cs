using Microsoft.AspNetCore.Mvc;
using EquipmentBorrowingSystem.Data;
using EquipmentBorrowingSystem.EMail;
using EquipmentBorrowingSystem.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace EquipmentBorrowingSystem.Controllers
{
    public class EquipmentReturnController : Controller
    {
        private readonly EmployeeContext _context;
        public EquipmentReturnController(EmployeeContext context)
        {
            _context = context;
        }


        //按下設備歸還，設備接收人必須填入個人的姓名 [代表為設備歸還的接收人(歸還)]
        public ActionResult Equipment_Receive_Person(string fOrderGuid)
        {
            //查詢此筆申請通過的借用紀錄
            var Application_Completed = _context.Application_Completed
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();




            return View(Application_Completed);/*傳送Application_Completed模型*/
        }



        // 建立設備摘要字典（每筆借用紀錄對應的設備名稱清單）
        private Dictionary<string, List<string>> BuildEquipmentSummary(IEnumerable<Application_Completed> records)
        {
            var orderGuids = records.Select(r => r.fOrderGuid).ToList();
            var allDetails = _context.Application_Details
                .Where(d => orderGuids.Contains(d.fOrderGuid))
                .ToList();
            return allDetails
                .GroupBy(d => d.fOrderGuid)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(d => d.Is_Consumable == "True"
                        ? $"{d.EName} {d.Emodel} x{d.Consumable_Borrowing_Times}"
                        : $"{d.EName} {d.Emodel} (#{d.EId})")
                        .ToList()
                );
        }

        public IActionResult Select_Equipment_BorrowingRecords()
        {
            var select_application_completed = _context.Application_Completed
                .Where(m => m.Status == "Returned")
                .OrderByDescending(m => m.Borrow_Time)
                .ToList();
            var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
            {
                Application_Completed = select_application_completed,
                EquipmentSummary = BuildEquipmentSummary(select_application_completed)
            };
            return View(ViewModel);
        }



        [HttpPost]
        public IActionResult Select_Equipment_BorrowingRecords(string IntputType, DateTime Date1, DateTime Date2, string Borrow_Name, string Equipment_Keyword)
        {
            List<Application_Completed> select_application_completed;
            string Date1String = null;
            string Date2String = null;

            // 設備名稱/型號查詢
            if (IntputType == "Equipment" && !string.IsNullOrEmpty(Equipment_Keyword))
            {
                // 先從 Application_Details 找到包含該設備的 fOrderGuid
                var matchingOrderGuids = _context.Application_Details
                    .Where(d => d.EName.Contains(Equipment_Keyword) || d.Emodel.Contains(Equipment_Keyword) || d.EId.Contains(Equipment_Keyword))
                    .Select(d => d.fOrderGuid)
                    .Distinct()
                    .ToList();

                select_application_completed = _context.Application_Completed
                    .Where(m => m.Status == "Returned" && matchingOrderGuids.Contains(m.fOrderGuid))
                    .OrderByDescending(m => m.Borrow_Time)
                    .ToList();

                var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
                {
                    Application_Completed = select_application_completed,
                    IntputType = IntputType,
                    Equipment_Keyword = Equipment_Keyword,
                    EquipmentSummary = BuildEquipmentSummary(select_application_completed)
                };
                return View(ViewModel);
            }

            // 借用人查詢
            if (!string.IsNullOrEmpty(Borrow_Name))
            {
                select_application_completed = _context.Application_Completed
                    .Where(m => m.Name.Contains(Borrow_Name) && m.Status == "Returned")
                    .OrderByDescending(m => m.Borrow_Time)
                    .ToList();
                var ViewModel = new Select_Equipment_BorrowingRecordsViewModel
                {
                    Application_Completed = select_application_completed,
                    IntputType = IntputType,
                    Borrow_Name = Borrow_Name,
                    EquipmentSummary = BuildEquipmentSummary(select_application_completed)
                };
                return View(ViewModel);
            }

            // 日期區間查詢
            DateTime date2 = Date2.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            Date1String = Date1.ToString("yyyy-MM-dd");
            Date2String = Date2.ToString("yyyy-MM-dd");

            if (IntputType == "ApplicationDate")
            {
                select_application_completed = _context.Application_Completed
                    .Where(m => m.Status == "Returned" && m.Date_Of_Application >= Date1 && m.Date_Of_Application <= date2)
                    .OrderByDescending(m => m.Borrow_Time)
                    .ToList();
            }
            else if (IntputType == "BorrowDate")
            {
                select_application_completed = _context.Application_Completed
                    .Where(m => m.Status == "Returned" && m.Borrow_Time >= Date1 && m.Borrow_Time <= date2)
                    .OrderByDescending(m => m.Borrow_Time)
                    .ToList();
            }
            else if (IntputType == "ReturnDate")
            {
                select_application_completed = _context.Application_Completed
                    .Where(m => m.Status == "Returned" && m.Return_Time >= Date1 && m.Return_Time <= date2)
                    .OrderByDescending(m => m.Borrow_Time)
                    .ToList();
            }
            else
            {
                return RedirectToAction("Select_Equipment_BorrowingRecords");
            }

            var viewModel = new Select_Equipment_BorrowingRecordsViewModel
            {
                Application_Completed = select_application_completed,
                IntputType = IntputType,
                Date1 = Date1String,
                Date2 = Date2String,
                EquipmentSummary = BuildEquipmentSummary(select_application_completed)
            };
            return View(viewModel);
        }




        [HttpPost]
        public ActionResult EquipmentReturn(string fOrderGuid, string Equipment_Recive_Person)
        {

            var select_application_completed = _context.Application_Completed
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();
            //判斷是否為返回建(因Equipment_Recive_Person.cshtml案返回鍵時會傳送fOrderGuid值)，如(Equipment_Recive_Person == null)代表是返回建
            if (Equipment_Recive_Person == null)
            {

                return RedirectToRoute(new { controller = "ApplicationCompleted", action = "ApplicationCompleted" });

            }
            //找尋此筆申請紀錄有哪些設備
            var select_application_details = _context.Application_Details
               .Where(m => m.fOrderGuid == fOrderGuid).ToList();


            //對fOrderGuid此筆申請紀錄的設備，借用及剩餘數量做變動，這裡是確認已歸還，因此(借用數量-1)(剩餘數量+1)

            foreach (var item1 in select_application_details)
            {
                var EName = item1.EName;
                var ESource = item1.ESource;
                var Emodel = item1.Emodel;
                var Is_Consumable = item1.Is_Consumable;
                var equipment = _context.Equipment
               .Where(m => m.EName == EName && m.ESource == ESource && m.Emodel == Emodel &&m.Is_Consumable== Is_Consumable).ToList();

                foreach (var item2 in equipment)
                {
                    if (item2.Is_Consumable == "False")
                    {
                        //將設備借用數量-1
                        var EBorrowing_Quantity = int.Parse(item2.EBorrowing_Quantity);
                        EBorrowing_Quantity--;
                        item2.EBorrowing_Quantity = EBorrowing_Quantity.ToString();
                        //將設備剩餘數量+1
                        var ERemaining_Quantity = int.Parse(item2.ERemaining_Quantity);
                        ERemaining_Quantity++;
                        item2.ERemaining_Quantity = ERemaining_Quantity.ToString();
                    }
                    else if (item2.Is_Consumable == "True")
                    {
                        //將消耗品借用數量-申請的借用數量
                        var EBorrowing_Quantity = int.Parse(item2.EBorrowing_Quantity);
                        EBorrowing_Quantity = EBorrowing_Quantity - int.Parse(item1.Consumable_Borrowing_Times);
                        item2.EBorrowing_Quantity = EBorrowing_Quantity.ToString();
                        //將消耗品剩餘數量+申請的借用數量
                        if (item2.ERemaining_Quantity != "∞")
                        {
                            var ERemaining_Quantity = int.Parse(item2.ERemaining_Quantity);
                            ERemaining_Quantity = ERemaining_Quantity + int.Parse(item1.Consumable_Borrowing_Times);
                            item2.ERemaining_Quantity = ERemaining_Quantity.ToString();
                        }
                          
                    }

                }
            }



            foreach (var item1 in select_application_details)
            {
                var EName = item1.EName;
                var EId = item1.EId;
                var ESource = item1.ESource;
                var EModel = item1.Emodel;
                //對fOrderGuid此筆申請紀錄的設備，作申請狀態及借用狀態的異動，這裡為歸還設備因此申請狀態及借用狀態為Fasle
                var equipment_details = _context.Equipment_Details
                   .Where(m => m.EName == EName && m.ESource == ESource && m.EId == EId && m.Emodel == EModel).ToList();
                foreach (var item2 in equipment_details)
                {
                    item2.ECurrent_Location = "數位內容設計研究中心";
                    item2.IsBorrow = "False";
                    item2.IsAddEquipment = "False";
                }
            }
            select_application_completed.FirstOrDefault().Equipment_Recive_Person = Equipment_Recive_Person;
            select_application_completed.FirstOrDefault().Status = "Returned";

            _context.SaveChanges();

            return RedirectToAction("Select_Equipment_BorrowingRecords");
        }


        //查詢借用紀錄的借用設備詳細資訊 //查詢借用紀錄的借用設備詳細資訊 //查詢借用紀錄的借用設備詳細資訊
        public ActionResult Select_Equipment_List(string fOrderGuid)
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



        //刪除申請列表以及列表內的詳細資料 //刪除申請列表以及列表內的詳細資料 //刪除申請列表以及列表內的詳細資料
        [HttpPost]
        public ActionResult Delete_Equipment_BorrowingRecords(string fOrderGuid)
        {

            //找到application資料表=fOrderGuid的內容
            var application = _context.Application
                .Where(m => m.fOrderGuid == fOrderGuid)
                .FirstOrDefault();


            //找到Application_Completed資料表=fOrderGuid的內容
            var Application_Completed = _context.Application_Completed
                .Where(m => m.fOrderGuid == fOrderGuid)
                .FirstOrDefault();



            // 找到所有 fOrderGuid
            var fOrderGuids = _context.Application_Details
                .Where(m => m.fOrderGuid == fOrderGuid)
                .ToList();
            foreach (var item in fOrderGuids)
            {

                var reduce_borrowing_times = _context.Count_Borrowing_Times
                    .Where(m => m.EName == item.EName && m.Emodel == item.Emodel)
                    .FirstOrDefault();

                if (int.Parse(reduce_borrowing_times.ECount_Borrowing_Times) > 1)
                {
                    reduce_borrowing_times.ECount_Borrowing_Times = (Convert.ToInt32(reduce_borrowing_times.ECount_Borrowing_Times) - 1).ToString();
                    _context.SaveChanges();
                }
                else if (int.Parse(reduce_borrowing_times.ECount_Borrowing_Times) == 1)
                {
                    _context.Count_Borrowing_Times.Remove(reduce_borrowing_times);
                    _context.SaveChanges();
                }
            }
            if (application != null)
            {
                _context.Application.Remove(application);
                _context.Application_Completed.Remove(Application_Completed);
                _context.SaveChanges();
                // 刪除所有 fOrderGuid
                foreach (var item in fOrderGuids)
                {
                    var application_details = _context.Application_Details
               .Where(m => m.fOrderGuid == item.fOrderGuid)
               .FirstOrDefault();
                    _context.Application_Details.Remove(application_details);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Select_Equipment_BorrowingRecords));
            }

            return RedirectToAction(nameof(Select_Equipment_BorrowingRecords));
        }

    }





}
