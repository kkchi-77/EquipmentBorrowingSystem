using Scaffoldong.Models;

namespace Scaffoldong.ViewModel
{
    public class Browse_equipment1ViewModel
    {
        public IEnumerable<Equipment> Equipment { get; set; }
      
        public string keyword { get; set; }
        public string select { get; set; }
    }
}
