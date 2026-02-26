namespace EquipmentBorrowingSystem.Models
{
    public class ApplicationViewModel
    {

        public IEnumerable<Application> ApplicationStatus { get; set; }
        public IEnumerable<Application_Details> ApplicationDetails { get; set; }
    }
}
