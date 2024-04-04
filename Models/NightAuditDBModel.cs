namespace Hotel_Core_MVC_V1.Models
{
    public class NightAuditDBModel
    {
        public string CheckInId { get; set; } = string.Empty;

        public string? BillNo { get; set; }

        public string? FolioCde { get; set; }

        public string? BillHDesp { get; set; }

        public DateTime BizDte { get; set; }

        public string RoomNo { get; set; } = string.Empty;

        public string? CustFullNme { get; set; }

        public decimal BillAmt { get; set; }

        public decimal BillDiscAmt { get; set; }

        public string CurrCde { get; set; } = string.Empty;

        public int? CurrRate { get; set; }

        public int? TaxAmt { get; set; }

        public string? SvrcCde { get; set; }

        public string? ChrgAccCde { get; set; }

        public string? PosId { get; set; }

        public string? DepCde { get; set; }

        public short ShiftNo { get; set; }

        // public int? VoidFlg { get; set; }

        //public int? VoidUserId { get; set; }

        //public DateTime? VoidDteTime { get; set; }

        public string? Remark { get; set; }

        public short Userid { get; set; }

        public short Cmpyid { get; set; }

        public DateTime Revdtetime { get; set; }
    }
}


