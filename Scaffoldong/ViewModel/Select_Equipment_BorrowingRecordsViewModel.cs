namespace Scaffoldong.ViewModel
{
    public class Select_Equipment_BorrowingRecordsViewModel
    {

        public IEnumerable<Application_Completed> Application_Completed { get; set; }
        public string IntputType { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }

        public string Borrow_Name { get; set; }
    }
}
