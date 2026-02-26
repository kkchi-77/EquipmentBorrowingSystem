using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Scaffoldong.Data;
using Scaffoldong.Migrations;
using Scaffoldong.Models;
using Scaffoldong.ViewModel;
using System.Security.Cryptography;
using System.IO;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
namespace Scaffoldong.Controllers
{
    public class Manage_All_EquipmentController : Controller
    {
        private readonly EmployeeContext _context;
        public Manage_All_EquipmentController(EmployeeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        //瀏覽所有設備器材//瀏覽所有設備器材//瀏覽所有設備器材
        public async Task<IActionResult> Browse_All_equipment()
        {
            var browse_equipment = await _context.Equipment
                .OrderBy(e => e.EName.Contains("VR頭盔") || e.EName.Contains("平板") ? 0 : 1)  // 將包含 "VR頭盔" 或 "平板" 的設備排在最前面
                .ThenBy(e => e.EName)  // 按照設備名稱的遞增排序
                .ThenBy(e => e.Emodel)  // 然後按照設備型號的遞減排序
                .ThenBy(e => e.ESource)  // 然後按照設備型號的遞減排序
                .ToListAsync();

            var viewModel = new Browse_equipment1ViewModel
            {
                Equipment = browse_equipment,
                select = "全部設備"
            };
            return _context.Equipment != null ?
                         View(viewModel) :
                         Problem("Entity set 'EmployeeContext.Manage_All_Equipment'  is null.");
        }


        [HttpPost]
        //瀏覽所有設備器材//瀏覽所有設備器材//瀏覽所有設備器材
        public async Task<IActionResult> Browse_All_equipment(string keyword, string select)
        {
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
                    return RedirectToAction("Browse_All_equipment");
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
            return RedirectToAction("Browse_All_equipment");
        }


        //根據選擇的設備名稱、設備型號、設備來源查看此設備的設備編號//根據選擇的設備名稱、設備型號、設備來源查看此設備的設備編號//根據選擇的設備名稱、設備型號、設備來源查看此設備的設備編號
        public ActionResult Browse_Equipment_EId(string EName, string Emodel, string ESource)
        {
            var browse_equipment_EId = _context.Equipment_Details
                   .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource)
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
        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EquipmentViewModel model)
        {
            if (model.FileInput != null && model.FileInput.Length > 0)
            {
                // 取得檔案名稱
                var fileName = Path.GetFileName(model.FileInput.FileName);
                // 指定檔案存儲路徑
                var filePath = Path.Combine("C:\\inetpub\\wwwroot\\EquipmentBorrow_System\\wwwroot\\Image", fileName);
                // 確認路徑中是否已存在相同檔案
                if (!System.IO.File.Exists(filePath))
                {
                    // 如果不存在相同檔案，保存新檔案
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.FileInput.CopyTo(stream);
                    }
                }

                // 將檔案保存到指定路徑
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.FileInput.CopyTo(stream);
                }
            }
            string EName = model.EName;
            string EModel = model.EModel;
            string Is_Consumable = model.Is_Consumable;
            string EQuantity = "";
            if (Is_Consumable == "True")
            {
                EQuantity = model.EQuantityNo;
            }
            else if (Is_Consumable == "False")
            {
                EQuantity = model.EQuantityYes;

            }

            List<string> equipmentNumbers = model.EId;
            string EQuantity_Unit = model.EQuantity_Unit;
            string ESource = model.ESource;
            string ECurrent_Location = model.ECurrent_Location;


            Equipment equipment = new Equipment();
            equipment.EName = EName;
            equipment.Emodel = EModel;
            equipment.EQuantity = EQuantity;
            equipment.EBorrowing_Quantity = "0";
            equipment.ERemaining_Quantity = EQuantity;
            equipment.EMissing_quantity = "0";
            equipment.EQuantity_Unit = EQuantity_Unit;
            equipment.ESource = ESource;
            if (model.FileInput != null && model.FileInput.Length > 0)
            {
                equipment.EImage = Path.GetFileName(model.FileInput.FileName);
            }
            else
            {
                equipment.EImage = " ";
            }
            equipment.Is_Consumable = Is_Consumable;
            _context.Equipment.Add(equipment);

            if (Is_Consumable == "False")
            {
                for (int i = 0; i < equipmentNumbers.Count; i++)
                {
                    string equipmentNumber = equipmentNumbers[i];
                    Equipment_Details equipment_Details = new Equipment_Details();
                    equipment_Details.EName = EName;
                    equipment_Details.Emodel = EModel;
                    equipment_Details.EId = equipmentNumber;
                    equipment_Details.ESource = ESource;
                    equipment_Details.IsBorrow = "False";
                    equipment_Details.ECurrent_Location = ECurrent_Location;
                    equipment_Details.IsAddEquipment = "False";
                    _context.Equipment_Details.Add(equipment_Details);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Browse_All_equipment");
        }

        [HttpPost]
        public ActionResult Delete(string EName, string Emodel, string ESource)
        {
            var equipment = _context.Equipment
        .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource)
        .FirstOrDefault();
            var equipment_details = _context.Equipment_Details
    .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource)
 .ToList();

            if (equipment != null || equipment_details != null)
            {
                _context.Equipment.Remove(equipment);
                _context.SaveChanges();
                foreach (var item in equipment_details)
                {
                    var equipment_details_delete = _context.Equipment_Details
                        .Where(m => m.EName == item.EName && m.Emodel == item.Emodel && m.ESource == item.ESource && m.EId == item.EId)
                        .FirstOrDefault();
                    _context.Equipment_Details.Remove(equipment_details_delete);
                    _context.SaveChanges();
                }
                return RedirectToAction("Browse_All_equipment");
            }
            return RedirectToAction("Browse_All_equipment");
        }


        [HttpPost]
        public ActionResult Update(string EName, string Emodel, string ESource, string Is_Consumable)
        {
            var equipment = _context.Equipment
        .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource)
        .ToList();

            var equipment_details = _context.Equipment_Details
     .Where(m => m.EName == EName && m.Emodel == Emodel && m.ESource == ESource)
     .ToList();

            // 自定義排序邏輯
            equipment_details = equipment_details.OrderBy(m =>
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
            var equipmentviewmodel = new EquipmentViewModel
            {
                Equipment = equipment,
                Equipment_Details = equipment_details,
                EName = EName,
                ESource = ESource,
                Is_Consumable = Is_Consumable
            };


            return View(equipmentviewmodel);
        }




        public ActionResult Update_Edit(EquipmentViewModel model)
        {
            if (model.FileInput != null && model.FileInput.Length > 0)
            {
                // 取得檔案名稱
                var fileName = Path.GetFileName(model.FileInput.FileName);
                // 指定檔案存儲路徑
                var filePath = Path.Combine("C:\\inetpub\\wwwroot\\EquipmentBorrow_System\\wwwroot\\Image", fileName);
                // 確認路徑中是否已存在相同檔案
                {
                    // 如果不存在相同檔案，保存新檔案
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.FileInput.CopyTo(stream);
                    }
                }

                // 將檔案保存到指定路徑
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.FileInput.CopyTo(stream);
                }
            }

            string EName = model.EName;
            string EName_old = model.EName_old;
            string EModel = model.EModel;
            string Emodel_old = model.Emodel_old;
            string EQuantity = model.EQuantity;
            List<string> EId = model.EId;
            List<string> IsBorrow = model.IsBorrow;
            List<string> ECurrent_Location_update = model.ECurrent_Location_update;
            string EQuantity_Unit = model.EQuantity_Unit;
            string ESource = model.ESource;
            string ESource_old = model.ESource_old;
            string EImage_old = model.EImage_old;
            string Is_Consumable = model.Is_Consumable;

            int eborrowing_quantity = 0;
            int emissing_quantity = 0;
            if (IsBorrow != null)
            {
                foreach (string value in IsBorrow)
                {
                    if (value == "True")
                    {
                        eborrowing_quantity++;
                    }
                    else if (value == "Missing")
                    {
                        emissing_quantity++;
                    }

                }
            }
            int eremaining_quantity = int.Parse(EQuantity) - eborrowing_quantity - emissing_quantity;
            string Eborrowing_Quantity = eborrowing_quantity.ToString();
            string ERemaining_Quantity = eremaining_quantity.ToString();
            string EMissing_Quantity = emissing_quantity.ToString();

            //----------更新Equipment資料表資料----------
            var equipment = _context.Equipment
            .Where(m => m.EName == EName_old && m.Emodel == Emodel_old && m.ESource == ESource_old)
            .FirstOrDefault();

            equipment.EName = EName;
            equipment.Emodel = EModel;
            equipment.EQuantity = EQuantity;
            equipment.EBorrowing_Quantity = Eborrowing_Quantity;
            equipment.ERemaining_Quantity = ERemaining_Quantity;
            equipment.EMissing_quantity = EMissing_Quantity;
            equipment.EQuantity_Unit = EQuantity_Unit;
            equipment.ESource = ESource;
            if (model.FileInput != null && model.FileInput.Length > 0)
            {
                equipment.EImage = Path.GetFileName(model.FileInput.FileName);
            }
            else
            {
                if (EImage_old == null)
                {
                    equipment.EImage = " ";
                }
                else
                {
                    equipment.EImage = EImage_old;
                }

            }
            equipment.Is_Consumable = Is_Consumable;
            //----------更新Equipment資料表資料----------


            //----------更新Equipment_Detail資料表資料----------
            if (Is_Consumable == "False")
            {
                var equipment_details = _context.Equipment_Details
.Where(m => m.EName == EName_old && m.Emodel == Emodel_old && m.ESource == ESource_old)
.ToList();
                if (EId != null)
                {
                    if (equipment_details.Count == EId.Count && equipment_details.Count == int.Parse(EQuantity))//設備總數量無變動，更改設備編號內容
                    {
                        for (int i = 0; i < equipment_details.Count; i++)
                        {
                            equipment_details[i].EName = EName;
                            equipment_details[i].Emodel = EModel;
                            equipment_details[i].EId = EId[i];
                            equipment_details[i].ESource = ESource;
                            equipment_details[i].IsBorrow = IsBorrow[i];
                            equipment_details[i].ECurrent_Location = ECurrent_Location_update[i];
                            equipment_details[i].IsAddEquipment = IsBorrow[i];
                        }
                    }
                    else if (equipment_details.Count < int.Parse(EQuantity))//增加設備總數量(增加不足的設備編號欄位)
                    {
                        for (int i = 0; i < int.Parse(EQuantity); i++)
                        {
                            if (i < equipment_details.Count)
                            {
                                equipment_details[i].EName = EName;
                                equipment_details[i].Emodel = EModel;
                                equipment_details[i].EId = EId[i];
                                equipment_details[i].ESource = ESource;
                                equipment_details[i].IsBorrow = IsBorrow[i];
                                equipment_details[i].ECurrent_Location = ECurrent_Location_update[i];
                                equipment_details[i].IsAddEquipment = IsBorrow[i];
                            }
                            else if (i >= equipment_details.Count)
                            {
                                Equipment_Details equipment_Details = new Equipment_Details();
                                equipment_Details.EName = EName;
                                equipment_Details.Emodel = EModel;
                                equipment_Details.EId = "無";
                                equipment_Details.ESource = ESource;
                                equipment_Details.IsBorrow = "False";
                                equipment_Details.ECurrent_Location = "數位內容設計研究中心";
                                equipment_Details.IsAddEquipment = "False";
                                _context.Equipment_Details.Add(equipment_Details);
                            }
                        }
                    }
                    else if (equipment_details.Count > int.Parse(EQuantity))//減少設備總數量(刪除多餘的設備編號欄位)
                    {
                        int equantity = equipment_details.Count - int.Parse(EQuantity);
                        var equipment_details_delet = _context.Equipment_Details
                        .Where(m => m.EName == EName && m.Emodel == EModel && m.ESource == ESource)
                        .Skip(int.Parse(EQuantity)) // 跳過前n筆資料
                        .Take(equantity) // 取後n筆資料
                        .ToList();
                        foreach (var item in equipment_details_delet)
                        {
                            _context.Equipment_Details.Remove(item);
                        }
                    }


                }
                else
                {
                    for (int i = 0; i < int.Parse(EQuantity); i++)
                    {
                        Equipment_Details equipment_Details = new Equipment_Details();
                        equipment_Details.EName = EName;
                        equipment_Details.Emodel = EModel;
                        equipment_Details.EId = "無";
                        equipment_Details.ESource = ESource;
                        equipment_Details.IsBorrow = "False";
                        equipment_Details.ECurrent_Location = "數位內容設計研究中心";
                        equipment_Details.IsAddEquipment = "False";
                        _context.Equipment_Details.Add(equipment_Details);
                    }

                }
            }


            //----------更新Equipment_Detail資料表資料----------


            // 保存更改到資料庫
            _context.SaveChanges();

            return RedirectToAction("Browse_All_equipment");
        }
    }

}
