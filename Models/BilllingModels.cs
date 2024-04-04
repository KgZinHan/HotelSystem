using System.ComponentModel.DataAnnotations;

namespace Hotel_Core_MVC_V1.Models
{
    public class BilllingModels
    {
    }

    public class BillingModel // Index
    {
        public long RoomLgId { get; set; }

        public string? CheckInId { get; set; }

        public int No { get; set; }

        public string? Roomno { get; set; }

        public string GuestName { get; set; } = string.Empty;

        public DateTime Occudte { get; set; }

        public DateTime Departdte { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal Balance { get; set; }
    }

    public class BillingView // View
    {

        public int BillId { get; set; }
        public int No { get; set; }

        public DateTime BizDte { get; set; }

        public string BillNo { get; set; } = string.Empty;

        public string BillDesp { get; set; } = string.Empty;

        public string RefNo2 { get; set; } = string.Empty;

        public string POSId { get; set; } = string.Empty;

        public string OutletId { get; set; } = string.Empty;

        public DateTime BillDateTime { get; set; }

        public bool? VoidFlg { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? Amount { get; set; }
    }

    public class PaymentBillingView // View
    {
        public string CheckInId { get; set; } = string.Empty;
        public string FolioCde { get; set; } = string.Empty;
        public string FolioName { get; set; } = string.Empty;
        public string FolioTotal { get; set; } = string.Empty;
        public bool CloseFolioFlg { get; set; } = false;
    }

    public class OtherBillingViewModels // View
    {
        public IEnumerable<BillingView> OtherFolioBills { get; set; } = new List<BillingView>();
        public PaymentBillingView PaymentBillingView { get; set; } = new PaymentBillingView();

    }

    public class BillingViewModels // View
    {
        public IEnumerable<BillingView> MainFolioBills { get; set; } = new List<BillingView>();
        public PaymentBillingView PaymentBillingView { get; set; } = new PaymentBillingView();
        public IEnumerable<OtherBillingViewModels> OtherFolioBillList { get; set; } = new List<OtherBillingViewModels>();

    }

    public class AddBillModel // Add
    {
        public string SrvcCde { get; set; } = string.Empty;

        public string? SrvcCdeDesc { get; set; } = string.Empty;

        public string? DeptCde { get; set; } = string.Empty;

        public string FolioCde { get; set; } = string.Empty;

        public int Qty { get; set; }

        public decimal Price { get; set; }

        public decimal Amount { get; set; }

        public string remark { get; set; } = string.Empty;
    }

    public class AddBillModels // Add
    {
        public CheckInModel CheckInModel { get; set; } = null!;

        public IEnumerable<AddBillModel> BillList { get; set; } = null!;
    }

}
