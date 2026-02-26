using Microsoft.AspNetCore.Mvc;
using Scaffoldong.Data;

namespace Scaffoldong.Controllers
{
    public class ApplicationRecordsController : Controller
    {

        private readonly EmployeeContext _context;
        public ApplicationRecordsController(EmployeeContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Select_ApplicationRecords(string fOrderguid, string credentials, string Assignor)
        {
            return View();
        }
    }
}
