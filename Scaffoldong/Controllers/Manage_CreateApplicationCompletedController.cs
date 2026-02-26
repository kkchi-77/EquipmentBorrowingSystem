using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scaffoldong.Data;
using System.Security.Claims;

namespace Scaffoldong.Controllers
{
    public class Manage_CreateApplicationCompletedController : Controller
    {
        private readonly EmployeeContext _context;
        public Manage_CreateApplicationCompletedController(EmployeeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //瀏覽可借用器材//瀏覽可借用器材//瀏覽可借用器材
        public async Task<IActionResult> Manage_CreateApplicationCompleted_Browse_equipment()
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
        public ActionResult Manage_CreateApplicationCompleted_Browse_equipment1(string keyword, string select)
        {
            // 判斷是否為登入狀態
            //if (SharedData.WelcomeMessage == null)
            //{
            //    return RedirectToAction("Login", "BorrowEquipment1");
            //}
            if (keyword != null)
            {
                var select_browse_equipment = _context.Equipment
                .Where(m => m.EName.Contains(keyword))
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
                    return RedirectToAction("Manage_CreateApplicationCompleted_Browse_equipment");
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
            return RedirectToAction("Manage_CreateApplicationCompleted_Browse_equipment");


        }

        //根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號
        public ActionResult Manage_CreateApplicationCompleted_Browse_Equipment_EId(string EName, string Emodel, string ESource)
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


        //根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號//根據選擇的設備名稱及設備來源查看此設備的設備編號
        public ActionResult Manage_CreateApplicationCompleted_Browse_Equipment_EId1(string EName, string Emodel, string ESource, string keyword, string select)
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
        public ActionResult Manage_CreateApplicationCompleted_Application_List()
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



            //var application_details = _context.Application_Details
            //    .Where(m => m.fUserId == fUserId && m.fIsApplied == "否")
            //    .OrderBy(m => m.Emodel) // 先按照 emodel 进行排序
            //    .ThenBy(m => m.ESource) // 然后按照 esource 进行排序
            //    .ToList();
            //View使用application_details模型
            return View(application_details);
        }


        //將申請清單內容新增至申請列表//將申請清單內容新增至申請列表//將申請清單內容新增至申請列表
        [HttpPost]
        public ActionResult Manage_CreateApplicationCompleted_Application_List(string Name, DateTime Borrow_Time, DateTime Return_Time, string Mobile, string fEmail, string Illustrate)
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
            application.Date_Of_Application = new DateTime(2000, 10, 10);
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
            return RedirectToAction("Manage_CreateApplicationCompleted_Application_Status");
        }



        //查詢申請列表//查詢申請列表//查詢申請列表
        public ActionResult Manage_CreateApplicationCompleted_Application_Status()
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




        //加入到申請清單//加入到申請清單//加入到申請清單
        public ActionResult Manage_CreateApplicationCompleted_AddEquipment(string EName, string Emodel, string EId, string ESource, string keyword, string select, string EBorrowing_Quantity, string Is_Consumable, string ERemaining_Quantity)
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
            if (previousPageUrl.Contains("Manage_CreateApplicationCompleted_Browse_Equipment_EId1"))
            {

                return RedirectToAction("Manage_CreateApplicationCompleted_Browse_Equipment_EId1", new { EName = EName, Emodel = Emodel, ESource = ESource, keyword = keyword, select = select });

            }
            else
            {
                if (Is_Consumable == "True")
                {
                    if (previousPageUrl.Contains("Manage_CreateApplicationCompleted_Browse_equipment1"))
                    {
                        return RedirectToAction("Manage_CreateApplicationCompleted_Browse_equipment1", new { keyword = keyword, select = select });
                    }
                    return RedirectToAction("Manage_CreateApplicationCompleted_Browse_equipment");
                }
                else
                {
                    return RedirectToAction("Manage_CreateApplicationCompleted_Browse_Equipment_EId", new { EName = EName, Emodel = Emodel, ESource = ESource });
                }
            }



        }





        //刪除還未提出申請的申請清單內容//刪除還未提出申請的申請清單內容//刪除還未提出申請的申請清單內容
        [HttpPost]

        public ActionResult Manage_CreateApplicationCompleted_Delete_Application_Details(int fid)
        {
            var application_details = _context.Application_Details
             .Where(m => m.fId == fid)
             .FirstOrDefault();
            if (application_details != null)
            {
                _context.Application_Details.Remove(application_details);
                _context.SaveChanges();
                return RedirectToAction(nameof(Manage_CreateApplicationCompleted_Application_List));
            }
            return RedirectToAction(nameof(Manage_CreateApplicationCompleted_Application_List));
        }


        //查詢申請清單詳細資訊//查詢申請清單詳細資訊//查詢申請清單詳細資訊
        public ActionResult Manage_CreateApplicationCompleted_Select_Application_List(string fOrderGuid)
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



        //接受此申請，接受人必須填入個人的姓名 [代表為設備移交人(借出)]
        public ActionResult Manage_CreateApplicationCompleted_Accept_application(string fOrderGuid)
        {

            var select_application = _context.Application
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();

            var application = new Models.Application();
            application.fOrderGuid = select_application.FirstOrDefault().fOrderGuid;
            application.Name = select_application.FirstOrDefault().Name;
            application.Borrow_Time = select_application.FirstOrDefault().Borrow_Time;
            application.Return_Time = select_application.FirstOrDefault().Return_Time;
            application.Mobile = select_application.FirstOrDefault().Mobile;
            application.fEmail = select_application.FirstOrDefault().fEmail;
            application.Illustrate = select_application.FirstOrDefault().Illustrate;
            return View(application);
        }


        //刪除申請列表以及列表內的詳細資料 //刪除申請列表以及列表內的詳細資料 //刪除申請列表以及列表內的詳細資料
        [HttpPost]
        public ActionResult Manage_CreateApplicationCompleted_Delete_Application_Status(string fOrderGuid)
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
                return RedirectToAction(nameof(Manage_CreateApplicationCompleted_Application_Status));
            }

            return RedirectToAction(nameof(Manage_CreateApplicationCompleted_Application_Status));
        }



    }
}
