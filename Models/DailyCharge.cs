namespace Hotel_Core_MVC_V1.Models
{
    public class DailyCharge
    {
        public int Id { get; set; }

        public string Date { get; set; }

        public string ServiceCode { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public int Qty { get; set; }

        public string Folio { get; set; } = string.Empty;

        public int FolioHId { get; set; }
    }

}
