using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using EquipmentBorrowingSystem.Data;
using EquipmentBorrowingSystem.Models;
using System.Security.Claims;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Razor.Language;
using FluentAssertions;
using System.Linq;
using Newtonsoft.Json.Linq;
using EquipmentBorrowingSystem.ViewModel;
using EquipmentBorrowingSystem.Migrations;
using System.Security.Cryptography;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace EquipmentBorrowingSystem.Controllers
{
    public class BorrowEquipment1Controller : Controller
    {
        private readonly EmployeeContext _context;
        public BorrowEquipment1Controller(EmployeeContext context)
        {
            _context = context;
        }
        //瀏覽可借用器材//瀏覽可借用器材//瀏覽可借用器材
        public async Task<IActionResult> Browse_equipment()
        {
            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}
            var equipmentList = await _context.Equipment
          .OrderBy(e => e.EName.Contains("VR頭盔") || e.EName.Contains("平板") ? 0 : 1)  // 將包含 "VR頭盔" 或 "平板" 的設備排在最前面
            .ThenBy(e => e.EName == "3號電池" ? 1 : (e.EName.Contains("傳輸線") ? 2 : 0))  // 將 "3號電池" 和包含 "傳輸線" 的設備排在最後面
          .ThenBy(e => e.EName)  // 按照設備名稱的遞增排序
          .ThenBy(e => e.Emodel)  // 然後按照設備型號的遞減排序
                    .ThenBy(e => e.ESource)  // 然後按照設備型號的遞減排序
          .ToListAsync();


            //這一段為之後優化要做的事
            //            var select_equipmentList_Remaining_Quantity = await _context.Equipment
            //    .Where(e => e.ERemaining_Quantity == "1")
            //    .ToListAsync();
            //            List<Equipment_Details> Equipment_Detail_1 = new List<Equipment_Details>();
            //            foreach (var item in select_equipmentList_Remaining_Quantity)
            //            {
            //                var Equipment_Detail = await _context.Equipment_Details
            //.Where(e => e.EName == item.EName && e.Emodel == item.Emodel && e.ESource == item.ESource)
            //.ToListAsync();
            //                Equipment_Detail.AddRange(Equipment_Detail);
            //            }

            return _context.Equipment != null ?
                          View(equipmentList) :
                          Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");
        }


        //根據查詢內容瀏覽可借用器材//根據查詢內容瀏覽可借用器材//根據查詢內容瀏覽可借用器材
        public ActionResult Browse_equipment1(string keyword, string select)
        {
            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}
            if (keyword != null)
            {
                var normalizedKeyword = keyword.Replace(" ", "").ToLower();

                var select_browse_equipment = _context.Equipment
                   .Where(m => m.EName.Replace(" ", "").ToLower().Contains(normalizedKeyword) ||
                               m.Emodel.Replace(" ", "").ToLower().Contains(normalizedKeyword))
                   .ToList();
                var viewModel = new Browse_equipment1ViewModel
                {
                    Equipment = select_browse_equipment,
                    keyword = keyword,
                    select = select
                };
                return select_browse_equipment != null ?
                          View(viewModel) :
                          Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");

            }
            else if (select != null)
            {
                if (select == "查詢全部")
                {
                    return RedirectToAction("Browse_equipment");
                }
                else
                {

                    if (select == "其他")
                    {
                        var select_browse_equipment = _context.Equipment
        .Where(m => !m.EName.Contains("平板") && !m.EName.Contains("頭盔") && !m.EName.Contains("筆電") && !m.EName.Contains("麥克風") && !m.EName.Contains("傳輸線") && !m.EName.Contains("轉接線"))
        .ToList();
                        var viewModel = new Browse_equipment1ViewModel
                        {
                            Equipment = select_browse_equipment,
                            keyword = keyword,
                            select = select
                        };
                        return select_browse_equipment != null ?
                                  View(viewModel) :
                                  Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");
                    }
                    else
                    {

                        var select_browse_equipment = _context.Equipment
                        .Where(m => m.EName.Contains(select))
                        .ToList();
                        var viewModel = new Browse_equipment1ViewModel
                        {
                            Equipment = select_browse_equipment,
                            keyword = keyword,
                            select = select
                        };
                        return select_browse_equipment != null ?
                                  View(viewModel) :
                                  Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");
                    }

                }
            }
            return RedirectToAction("Browse_equipment");


        }

        //根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號
        public ActionResult Browse_Equipment_EId(string EName, string Emodel, string ESource)
        {

            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}



            var browse_equipment_EId = _context.Equipment_Details
                   .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource && m.IsBorrow == "False" && m.IsAddEquipment == "False")
                    .OrderBy(m => m.EId)
                   .ToList();

            // 自定義排序邏輯
            browse_equipment_EId = browse_equipment_EId.OrderBy(m =>
            {
                if (int.TryParse(m.EId, out int result)) // 如果能夠成功轉換為整數，則按照整數排序
                {
                    return result;
                }
                else // 否則按照字符串排序
                {
                    return int.MaxValue; // 將非數字的字符串排在最後
                }
            })
            .ToList();

            return browse_equipment_EId != null ?
                      View(browse_equipment_EId) :
                      Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");


        }


        // 回傳可選編號清單（供 Modal AJAX 使用）
        [HttpPost]
        public IActionResult GetEquipmentEIds(string EName, string Emodel, string ESource)
        {
            var eIds = _context.Equipment_Details
                .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource
                            && m.IsBorrow == "False" && m.IsAddEquipment == "False")
                .OrderBy(m => m.EId)
                .ToList()
                .OrderBy(m => int.TryParse(m.EId, out int r) ? r : int.MaxValue)
                .Select(m => new { m.EId, m.EName, m.Emodel, m.ESource })
                .ToList();
            return Json(eIds);
        }

        //根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號
        public ActionResult Browse_Equipment_EId1(string EName, string Emodel, string ESource, string keyword, string select)
        {

            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}



            var browse_equipment_EId = _context.Equipment_Details
                   .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource && m.IsBorrow == "False" && m.IsAddEquipment == "False")
                    .OrderBy(m => m.EId)
                   .ToList();

            // 自定義排序邏輯
            browse_equipment_EId = browse_equipment_EId.OrderBy(m =>
            {
                if (int.TryParse(m.EId, out int result)) // 如果能夠成功轉換為整數，則按照整數排序
                {
                    return result;
                }
                else // 否則按照字符串排序
                {
                    return int.MaxValue; // 將非數字的字符串排在最後
                }
            })
            .ToList();
            var browse_equipment_EId1 = new Browse_Equipment_EId1ViewModel
            {
                Equipment_Details = browse_equipment_EId,
                keyword = keyword,
                select = select
            };

            return browse_equipment_EId1 != null ?
                      View(browse_equipment_EId1) :
                      Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");


        }


        //申請清單內容 //申請清單內容 //申請清單內容
        public ActionResult Application_List()
        {
            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}

            //取得登入會員的帳號並指定給fUserId
            var fUserId1 = _context.tMember
   .Where(m => m.fUserId == "Kao77")
   .FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代
            //找出未成為申請明細的資料，即申請清單內容
            var application_details = _context.Application_Details
        .Where(m => m.fUserId == fUserId1.fUserId && m.fIsApplied == "否")
                  .ToList();

            // 自定義排序邏輯
            application_details = application_details
                .OrderBy(m => m.Emodel) // 先按照 emodel 进行排序
                .ThenBy(m => m.ESource) // 然后按照 esource 进行排序
                .ThenBy(m =>
                {
                    if (int.TryParse(m.EId, out int result)) // 如果能夠成功轉換為整數，則按照整數排序
                    {
                        return result;
                    }
                    else // 否則按照字符串排序
                    {
                        return int.MaxValue; // 將非數字的字符串排在最後
                    }
                })
                .ToList();
   

            return View(application_details);
        }


        //將申請清單內容新增至申請列表//將申請清單內容新增至申請列表//將申請清單內容新增至申請列表
        [HttpPost]
        public ActionResult Application_List(string Name, DateTime Borrow_Time, DateTime Return_Time, string Mobile, string fEmail, string Illustrate)
        {
            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}

            //找出會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;

            var fUserId1 = _context.tMember
     .Where(m => m.fUserId == "Kao77")
     .FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代

            //建立唯一的識別值並指定給guid變數，用來當做訂單編號
            //Application的fOrderGuid欄位會關聯到Application_Details的fOrderGuid欄位
            //形成一對多的關係，即一筆訂單資料會對應到多筆訂單明細
            string guid = Guid.NewGuid().ToString();
            //建立訂單主檔資料

            Models.Application application = new Models.Application();
            application.Date_Of_Application = DateTime.Now;
            application.fOrderGuid = guid;
            application.fUserId = fUserId1.fUserId;
            application.Name = Name;
            application.Borrow_Time = Borrow_Time;
            application.Return_Time = Return_Time;
            application.Mobile = Mobile;
            application.fEmail = fEmail;
            application.Illustrate = Illustrate;
            application.Status = "Applying";
            _context.Application.Add(application);
            //找出未成為申請明細的資料，即申請清單內容
            var application_details = _context.Application_Details
                .Where(m => m.fIsApplied == "否" && m.fUserId == fUserId1.fUserId)
                .ToList();


            //將購物車狀態產品的fIsApproved設為"是"，表示確認訂購產品
            foreach (var item in application_details)
            {
                item.fOrderGuid = guid;
                item.fIsApplied = "是";
            }
            //更新資料庫，異動tOrder和tOrderDetail
            //完成訂單主檔和訂單明細的更新
            _context.SaveChanges();
            return RedirectToAction("Application_Status");
        }

        //加入到申請清單//加入到申請清單//加入到申請清單
        public ActionResult AddEquipment(string EName, string Emodel, string EId, string ESource, string keyword, string select, string EBorrowing_Quantity, string Is_Consumable, string ERemaining_Quantity)
        {
            // 獲取上一頁的 URL
            string previousPageUrl = HttpContext.Request.Headers["Referer"];


            //取得會員帳號並指定給fUserId
            string fUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var fUserId1 = _context.tMember
     .Where(m => m.fUserId == "Kao77")
     .FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代

            //找出會員放入訂單明細的產品，該產品的fIsApproved為"否"
            //表示該產品是購物車狀態
            var current_AddEquipment = _context.Application_Details
                .Where(m => m.fUserId == fUserId1.fUserId && m.fIsApplied == "否" && m.EName == EName && m.Emodel == Emodel && m.EId == EId && m.Is_Consumable == "False")
                .FirstOrDefault();

            //表示該消耗品是購物車狀態
            var current_AddConsumables = _context.Application_Details
                .Where(m => m.fUserId == fUserId1.fUserId && m.fIsApplied == "否" && m.EName == EName && m.Emodel == Emodel && m.Is_Consumable == "True")
                .FirstOrDefault();

            //用來判斷新增消耗品時，累計的借用數量是否已達上限
            bool exceedLimit = false;

            bool Is_insert_consumables = false;





            //若currentCar等於null，表示會員選購的產品不是購物車狀態
            if (current_AddEquipment == null && Is_Consumable == null)
            {//將產品放入訂單明細，因為產品的fIsApproved為"否"，表示為購物車狀態
                Application_Details application_details = new Application_Details();
                application_details.fOrderGuid = " ";
                application_details.fUserId = fUserId1.fUserId;
                application_details.EName = EName;
                application_details.Emodel = Emodel;
                application_details.EId = EId;
                application_details.fIsApplied = "否";
                application_details.ESource = ESource;
                application_details.IsCount_Borrowing_Times = "否";
                application_details.Consumable_Borrowing_Times = "1";
                application_details.Is_Consumable = "False";
                _context.Application_Details.Add(application_details);


            }
            //若current_AddConsumables等於null，表示消耗品不是購物車狀態，反之，則是購物車狀態，且必須要將數量加總
            if (current_AddConsumables == null && Is_Consumable == "True")
            {
                //將產品放入訂單明細，因為產品的fIsApproved為"否"，表示為購物車狀態
                Application_Details application_details = new Application_Details();
                application_details.fOrderGuid = " ";
                application_details.fUserId = fUserId1.fUserId;
                application_details.EName = EName;
                application_details.Emodel = Emodel;
                application_details.EId = "消耗品";
                application_details.fIsApplied = "否";
                application_details.ESource = ESource;
                application_details.IsCount_Borrowing_Times = "否";
                application_details.Consumable_Borrowing_Times = EBorrowing_Quantity;
                application_details.Is_Consumable = "True";
                _context.Application_Details.Add(application_details);
                Is_insert_consumables = true;

            }
            else if ((current_AddConsumables != null && Is_Consumable == "True"))
            {
                var equipment_Consumables = _context.Equipment
                .Where(m => m.EName == current_AddConsumables.EName && m.Emodel == current_AddConsumables.Emodel && m.ESource == current_AddConsumables.ESource)
                .FirstOrDefault();
                if (equipment_Consumables.ERemaining_Quantity != "∞")
                {
                    if ((int.Parse(current_AddConsumables.Consumable_Borrowing_Times) + int.Parse(EBorrowing_Quantity)) > int.Parse(ERemaining_Quantity))
                    {
                        exceedLimit = true;
                    }
                    else
                    {
                        current_AddConsumables.Consumable_Borrowing_Times = (int.Parse(current_AddConsumables.Consumable_Borrowing_Times) + int.Parse(EBorrowing_Quantity)).ToString();
                        Is_insert_consumables = true;
                    }
                }
                else
                {
                    current_AddConsumables.Consumable_Borrowing_Times = (int.Parse(current_AddConsumables.Consumable_Borrowing_Times) + int.Parse(EBorrowing_Quantity)).ToString();
                    Is_insert_consumables = true;
                }


            }
            //else
            //{
            //    // 將字串轉換成數字(設備數量)
            //    int currentEQuantity = int.Parse(current_AddEquipment.EQuantity);
            //    int eQuantity = int.Parse(EQuantity);


            //    // 將新增的設備數量+原新增的設備數量
            //    int result = currentEQuantity + eQuantity;

            //    //如果result的值大於設備總數量
            //    if (result > Equipment_EQuantity)
            //    {
            //        result = Equipment_EQuantity;
            //    }
            //    // 將數字轉換成字串
            //    string resultString = result.ToString();
            //    //若產品為購物車狀態，即將該產品數量加
            //    current_AddEquipment.EQuantity = resultString;
            //}
            _context.SaveChanges();
            TempData["ExceedLimit"] = exceedLimit;
            TempData["Is_insert_consumables"] = Is_insert_consumables;
            TempData["EName"] = EName;
            TempData["Emodel"] = Emodel;

            TempData["EBorrowing_Quantity"] = EBorrowing_Quantity;
            // 判斷 previousPageUrl 是否包含 "Browse_equipment1 !! 代表者搜尋過後的頁面"
            if (previousPageUrl.Contains("Browse_Equipment_EId1"))
            {

                return RedirectToAction("Browse_Equipment_EId1", new { EName = EName, Emodel = Emodel, ESource = ESource, keyword = keyword, select = select });

            }
            else
            {
                if (Is_Consumable == "True")
                {
                    if (previousPageUrl.Contains("Browse_equipment1"))
                    {
                        return RedirectToAction("Browse_equipment1", new { keyword = keyword, select = select });
                    }
                    return RedirectToAction("Browse_equipment");
                }
                else
                {
                    return RedirectToAction("Browse_Equipment_EId", new { EName = EName, Emodel = Emodel, ESource = ESource });
                }
            }


        }


        //當此設備剩餘數量為1，省略選擇此設備編號按鈕，直接添加至申請清單
        public ActionResult AddEquipment_ERemaining_Quantity_1(string EName, string Emodel, string ESource)
        {
            //取得會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;

            var fUserId1 = _context.tMember
.Where(m => m.fUserId == "Kao77")
.FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代


            //找出會員放入訂單明細的產品，該產品的fIsApproved為"否"
            //表示該產品是購物車狀態
            var current_AddEquipment = _context.Application_Details
                .Where(m => m.fUserId == fUserId1.fUserId && m.fIsApplied == "否" && m.EName == EName)
                .FirstOrDefault();


            var equipment_details = _context.Equipment_Details
                .Where(m => m.EName == EName && m.Emodel == Emodel && m.IsBorrow == "False" && m.ESource == ESource)
                .FirstOrDefault();

            //若currentCar等於null，表示會員選購的產品不是購物車狀態
            if (current_AddEquipment == null)
            {
                //給予申請編號，讓資料表形成1對多的關係
                string guid = Guid.NewGuid().ToString();
                //找出目前選購的產品並指定給product
                var product = _context.Application_Details.Where(m => m.EName == EName).FirstOrDefault();


                //將產品放入訂單明細，因為產品的fIsApproved為"否"，表示為購物車狀態
                Application_Details application_details = new Application_Details();
                application_details.fOrderGuid = "";
                application_details.fUserId = fUserId1.fUserId;
                application_details.EName = EName;
                application_details.Emodel = Emodel;
                application_details.EId = equipment_details.EId;
                application_details.fIsApplied = "否";
                application_details.ESource = ESource;
                _context.Application_Details.Add(application_details);
            }

            _context.SaveChanges();
            return RedirectToAction("Browse_equipment");
        }





        //查詢申請列表//查詢申請列表//查詢申請列表
        public ActionResult Application_Status()
        {

            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}
            //找出會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;

            var fUserId1 = _context.tMember
.Where(m => m.fUserId == "Kao77")
.FirstOrDefault(); //抓fUserId問題暫時無法解決，先用硬解替代


            //找出目前所有申請主檔記錄並依照Borrow_Time進行遞增排序
            //將查詢結果指定給orders
            var application_Status = _context.Application.Where(m => m.fUserId == fUserId1.fUserId)
                .OrderByDescending(m => m.Date_Of_Application).ToList();
            //目前所有申請主檔Application_Status.cshtml檢視使用application_Status模型
            return View(application_Status);
        }

        //查詢申請清單詳細資訊//查詢申請清單詳細資訊//查詢申請清單詳細資訊
        public ActionResult Select_Application_List(string fOrderGuid)
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








        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("fId,fUserId,fPwd,fName,fEmail")] tMember tmember)
        {

            if (ModelState.IsValid == false)
            {
                return View();
            }
            // 依帳號取得會員並指定給member
            var memberUserId = _context.tMember
                .Where(m => m.fUserId == tmember.fUserId)
                .FirstOrDefault();
            var memberPwd = _context.tMember
                .Where(m => m.fPwd == tmember.fPwd)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (memberUserId != null)
            {
                ViewBag.Message = "此帳號己有人使用，註冊失敗";
                return View();
            }
            else if (memberPwd != null)
            {
                ViewBag.Message = "此密碼己有人使用，註冊失敗";
                return View();
            }
            else if (memberUserId != null && memberPwd != null)
            {
                ViewBag.Message = "帳號或密碼已有人使用，註冊失敗";
                return View();
            }
            else
            {
                //將會員記錄新增到tMember資料表
                _context.Add(tmember);
                await _context.SaveChangesAsync();
                //執行Home控制器的Login動作方法
                return RedirectToAction(nameof(Index));
            }
            /*if (ModelState.IsValid)
            {

                _context.Add(tmember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tmember);*/
        }
        public IActionResult Login()
        {
            return View();
        }
        //Post: Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string fUserId, string fPwd)
        {
            // 依帳密取得會員並指定給member
            var member = _context.tMember
     .AsEnumerable()
     .Where(m => string.Equals(m.fUserId, fUserId, StringComparison.Ordinal) &&
                 string.Equals(m.fPwd, fPwd, StringComparison.Ordinal))
     .FirstOrDefault();


            //若member為null，表示會員未註冊
            if (member == null)
            {
                ViewBag.Message = "登入失敗，帳號或密碼錯誤，請重新輸入！";
                return View();
            }

            // 使用Cookie认证记录用户信息
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, fUserId),// 可以根据需要添加其他的用户信息
            
        };
            ///判斷是否登入///判斷是否登入///判斷是否登入
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // 是否持久化Cookie
                IsPersistent = true,
                // 设置滑动过期
                AllowRefresh = true,
                // 设置滑动过期时间为365天
                ExpiresUtc = null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            SharedData.WelcomeMessage = fUserId + " 歡迎光臨";
            ///判斷是否登入///判斷是否登入///判斷是否登入



            return RedirectToAction("Browse_equipment", "BorrowEquipment1");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // 清除其他可能存储的用户相关信息 
            // 判斷是否為登入狀態
            SharedData.WelcomeMessage = null;

            return RedirectToAction("Login", "BorrowEquipment1");
        }




        //刪除申請列表以及列表內的詳細資料 //刪除申請列表以及列表內的詳細資料 //刪除申請列表以及列表內的詳細資料
        [HttpPost]
        public ActionResult Delete_Application_Status(string fOrderGuid)
        {
            if (_context.Application_Details == null)
            {
                return Problem("Entity set 'EmployeeContext.Employee'  is null.");
            }
            //找到application資料表=fOrderGuid的內容
            var application = _context.Application
                .Where(m => m.fOrderGuid == fOrderGuid)
                .FirstOrDefault();
            // 找到所有 fOrderGuid
            var fOrderGuids = _context.Application_Details
                .Where(m => m.fOrderGuid == fOrderGuid)
                .Select(m => m.fOrderGuid)
                .ToList();

            if (application != null)
            {
                _context.Application.Remove(application);
                _context.SaveChanges();
                // 刪除所有 fOrderGuid
                foreach (var item in fOrderGuids)
                {
                    var application_details = _context.Application_Details
               .Where(m => m.fOrderGuid == item)
               .FirstOrDefault();
                    _context.Application_Details.Remove(application_details);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Application_Status));
            }

            return RedirectToAction(nameof(Application_Status));
        }


        //刪除還未提出申請的申請清單內容//刪除還未提出申請的申請清單內容//刪除還未提出申請的申請清單內容
        [HttpPost]

        public ActionResult Delete_Application_Details(int fid)
        {
            var application_details = _context.Application_Details
             .Where(m => m.fId == fid)
             .FirstOrDefault();
            if (application_details != null)
            {
                _context.Application_Details.Remove(application_details);
                _context.SaveChanges();
                return RedirectToAction(nameof(Application_List));
            }
            return RedirectToAction(nameof(Application_List));
        }





        //接受此申請，接受人必須填入個人的姓名 [代表為設備移交人(借出)]

        //修改申請清單，消耗品的數量
        [HttpPost]
        public ActionResult UpdateConsumableBorrowingTimes(string EName, string Emodel, int newBorrowingTimes,string fOrderGuid)
        {
            var applicationDetail = _context.Application_Details
             .Where(m => m.EName == EName && m.Emodel == Emodel&& m.fOrderGuid == " ")
             .FirstOrDefault();
            if (applicationDetail != null)
            {
                applicationDetail.Consumable_Borrowing_Times = newBorrowingTimes.ToString();
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        //取得申請清單中，消耗品的設備數量
        public ActionResult GetEquipmentQuantity(string EName, string Emodel)
        {
            var equipment = _context.Equipment
                .Where(m => m.EName == EName && m.Emodel == Emodel && m.Is_Consumable == "True")
                .FirstOrDefault();

            if (equipment != null)
            {
                return Json(new { quantity = equipment.ERemaining_Quantity }); // 替换为实际的数量字段
            }

            return Json(new { quantity = 0 });
        }
        ////已接受此申請 //已接受此申請 //已接受此申請
        //public ActionResult ApplicationCompleted(string fOrderGuid, string Credentials_Mortgage, string Equipment_Handover_Person)
        //{
        //    string fUserId = User.Identity.Name;
        //    var select_application = _context.Application
        //        .Where(m => m.fOrderGuid == fOrderGuid).ToList();





        //    Application_Completed application_completed = new Application_Completed();
        //    application_completed.fOrderGuid = fOrderGuid;
        //    application_completed.fUserId = fUserId;
        //    application_completed.Name = select_application.FirstOrDefault().Name;
        //    application_completed.Borrow_Time = select_application.FirstOrDefault().Borrow_Time;
        //    application_completed.Return_Time = select_application.FirstOrDefault().Return_Time;
        //    application_completed.Mobile = select_application.FirstOrDefault().Mobile;
        //    application_completed.fEmail = select_application.FirstOrDefault().fEmail;
        //    application_completed.Illustrate = select_application.FirstOrDefault().Illustrate;
        //    application_completed.Status = "True";
        //    application_completed.Credentials_Mortgage = Credentials_Mortgage;
        //    application_completed.Equipment_Handover_Person = Equipment_Handover_Person;
        //    _context.Application_Completed.Add(application_completed);




        //    _context.SaveChanges();
        //    return View();
        //}

    }
}
