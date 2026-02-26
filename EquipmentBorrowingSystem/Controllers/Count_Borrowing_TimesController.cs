using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipmentBorrowingSystem.Data;

using EquipmentBorrowingSystem.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FluentAssertions;
namespace EquipmentBorrowingSystem.Controllers
{
    public class Count_Borrowing_TimesController : Controller
    {
        private readonly EmployeeContext _context;
        public Count_Borrowing_TimesController(EmployeeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Count_Borrowing_Times()
        {
            var selectedApplication_Details = _context.Application_Details
                .ToList();
            _context.Database.ExecuteSqlRaw("DELETE FROM Count_Borrowing_Times");
            for (int i = 0; i < selectedApplication_Details.Count; i++)
            {
                var count_borrowing_times = _context.Count_Borrowing_Times
                    .FirstOrDefault(m => m.EName == selectedApplication_Details[i].EName && m.Emodel == selectedApplication_Details[i].Emodel);

                var selectedEquipment_Details = _context.Equipment_Details
                    .FirstOrDefault(m => m.EName == selectedApplication_Details[i].EName && m.Emodel == selectedApplication_Details[i].Emodel && m.ESource == selectedApplication_Details[i].ESource && m.EId == selectedApplication_Details[i].EId);

                var selectedApplication_Completed = _context.Application_Completed
                    .FirstOrDefault(m => m.fOrderGuid == selectedApplication_Details[i].fOrderGuid);

                if (count_borrowing_times == null && selectedApplication_Completed != null)
                {

                    // Create a new Count_Borrowing_Times object
                    Count_Borrowing_Times newCountBorrowingTimes = new Count_Borrowing_Times
                    {
                        EName = selectedApplication_Details[i].EName,
                        Emodel = selectedApplication_Details[i].Emodel,
                        ECount_Borrowing_Times = selectedApplication_Details[i].Consumable_Borrowing_Times
                    };
                    selectedApplication_Details[i].IsCount_Borrowing_Times = "是";
                    // Add the new object to the context and save changes
                    _context.Count_Borrowing_Times.Add(newCountBorrowingTimes);
                    _context.SaveChanges();
                }
                else if (count_borrowing_times != null && selectedApplication_Completed != null)
                {
                    if (selectedApplication_Details[i].Is_Consumable == "False")
                    {
                        // Increment the borrowing times count
                        count_borrowing_times.ECount_Borrowing_Times = (int.Parse(count_borrowing_times.ECount_Borrowing_Times) + 1).ToString();
                    }
                    else
                    {
                        // Increment the borrowing times count
                        count_borrowing_times.ECount_Borrowing_Times = (int.Parse(count_borrowing_times.ECount_Borrowing_Times) + int.Parse(selectedApplication_Details[i].Consumable_Borrowing_Times)).ToString();
                    }
      


                    selectedApplication_Details[i].IsCount_Borrowing_Times = "是";
                    // Save changes to the existing object
                    _context.SaveChanges();
                }
            }
            var count_borrowing_times1 = _context.Count_Borrowing_Times
      .OrderByDescending(m => Convert.ToInt32(m.ECount_Borrowing_Times))
      .ToList();
            return View(count_borrowing_times1);
        }


        public ActionResult Count_Borrowing_Times_selectdate(DateTime Date1, DateTime Date2)
        {
            // 在後端代碼中處理日期
            DateTime date2 = Convert.ToDateTime(Request.Form["Date2"]);
            date2 = date2.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            //查詢Date1~Date2區間的設備借用紀錄
            var selectedApplication_Completed1 = _context.Application_Completed
.Where(m => m.Borrow_Time >= Date1 && m.Borrow_Time <= date2)
                .ToList();

            //刪除Count_Borrowing_Times資料表資料，每次重新統計計算
            _context.Database.ExecuteSqlRaw("DELETE FROM Count_Borrowing_Times");
            if (selectedApplication_Completed1 != null)//判斷selectedApplication_Completed1，是否有Date1~Date2區間的設備借用紀錄
            {
                foreach (var item in selectedApplication_Completed1)
                {
                    var selectedApplication_Details = _context.Application_Details
                        .Where(m => m.fOrderGuid == item.fOrderGuid)
                    .ToList();
                    for (int i = 0; i < selectedApplication_Details.Count; i++)
                    {
                        var count_borrowing_times = _context.Count_Borrowing_Times
                            .FirstOrDefault(m => m.EName == selectedApplication_Details[i].EName && m.Emodel == selectedApplication_Details[i].Emodel);

                        var selectedEquipment_Details = _context.Equipment_Details
                            .FirstOrDefault(m => m.EName == selectedApplication_Details[i].EName && m.Emodel == selectedApplication_Details[i].Emodel && m.ESource == selectedApplication_Details[i].ESource && m.EId == selectedApplication_Details[i].EId);

                        var selectedApplication_Completed = _context.Application_Completed
                            .FirstOrDefault(m => m.fOrderGuid == selectedApplication_Details[i].fOrderGuid);

                        if (count_borrowing_times == null && selectedApplication_Completed != null)
                        {
                            // Create a new Count_Borrowing_Times object
                            Count_Borrowing_Times newCountBorrowingTimes = new Count_Borrowing_Times
                            {
                                EName = selectedApplication_Details[i].EName,
                                Emodel = selectedApplication_Details[i].Emodel,
                                ECount_Borrowing_Times = "1"
                            };
                            selectedApplication_Details[i].IsCount_Borrowing_Times = "是";
                            // Add the new object to the context and save changes
                            _context.Count_Borrowing_Times.Add(newCountBorrowingTimes);
                            _context.SaveChanges();
                        }
                        else if (count_borrowing_times != null && selectedApplication_Completed != null)
                        {
                            // Increment the borrowing times count
                            count_borrowing_times.ECount_Borrowing_Times = (int.Parse(count_borrowing_times.ECount_Borrowing_Times) + 1).ToString();


                            selectedApplication_Details[i].IsCount_Borrowing_Times = "是";
                            // Save changes to the existing object
                            _context.SaveChanges();
                        }
                    }




                }
                var count_borrowing_times1 = _context.Count_Borrowing_Times
  .OrderByDescending(m => Convert.ToInt32(m.ECount_Borrowing_Times))
  .ToList();
                string Date1String = Date1.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                string Date2String = Date2.ToString("yyyy-MM-dd"); // 這裡使用了 "yyyy-MM-dd" 格式字串
                var ViewModel = new Count_Borrowing_Times_selectdateViewModel
                {
                    Count_Borrowing_Times = count_borrowing_times1,
                    Date1 = Date1String,
                    Date2 = Date2String
                };
                return View(ViewModel);
            }


            return View();
        }


    }
}