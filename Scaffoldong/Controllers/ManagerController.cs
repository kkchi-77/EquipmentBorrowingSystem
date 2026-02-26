using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scaffoldong.Data;
using System.Security.Claims;

namespace Scaffoldong.Controllers
{
    public class ManagerController : Controller
    {
        private readonly EmployeeContext _context;
        private readonly IConfiguration _configuration;

        public ManagerController(EmployeeContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            // 判斷是否為登入狀態
            if (SharedData.WelcomeMessage == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            return _context.BorrowEquipment1 != null ?
                          View(await _context.BorrowEquipment1.ToListAsync()) :
                          Problem("Entity set 'EmployeeContext.BorrowEquipment1'  is null.");
        }

        public IActionResult Login()
        {
            return View();
        }

        //Post: Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string fUserId, string fPwd)
        {
            // 依帳密取得會員並指定給manager
            var manager = _context.tManager
                .AsEnumerable()
                .Where(m => string.Equals(m.fManagerID, fUserId, StringComparison.Ordinal) &&
                            string.Equals(m.fManagerPwd, fPwd, StringComparison.Ordinal))
                .FirstOrDefault();

            //若manager為null，表示帳密錯誤
            if (manager == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }

            // 使用 Cookie 記錄登入狀態
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, fUserId),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            SharedData.WelcomeMessage = "管理者你好";
            return RedirectToAction("ApplicationCompleted", "ApplicationCompleted");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            SharedData.WelcomeMessage = null;
            return RedirectToAction("Login", "Manager");
        }

        /// <summary>
        /// 驗證管理者操作密碼（供前端 AJAX 呼叫，密碼不再暴露於前端程式碼）
        /// </summary>
        [HttpPost]
        public IActionResult ValidateAdminPassword([FromBody] ValidateAdminPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Password))
                return Json(new { success = false });

            string adminActionPassword = _configuration["AdminActionPassword"] ?? string.Empty;

            bool isValid = string.Equals(request.Password, adminActionPassword, StringComparison.Ordinal);
            return Json(new { success = isValid });
        }
    }

    /// <summary>
    /// 管理者操作密碼驗證請求 DTO
    /// </summary>
    public class ValidateAdminPasswordRequest
    {
        public string Password { get; set; }
    }
}
