namespace EquipmentBorrowingSystem.ViewModel
{
    public class Select_Equipment_BorrowingRecordsViewModel
    {

        public IEnumerable<Application_Completed> Application_Completed { get; set; }
        public string IntputType { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }

        public string Borrow_Name { get; set; }
        public string Equipment_Keyword { get; set; }

        // 每筆紀錄對應的設備摘要（key: fOrderGuid, value: 設備名稱清單）
        public Dictionary<string, List<string>> EquipmentSummary { get; set; }
    }
}
