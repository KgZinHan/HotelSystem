using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Permissions;

namespace Hotel_Core_MVC_V1.Models
{
    public class ReportsModel
    {
        public string ReportTitle { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }

        public int ToNext { get; set; }

        public int ReportTitleNo { get; set; }
    }

    public class OccupancyReportModel
    {
        public int billd { get; set; }

        public string Checkinid { get; set; } = null!;

        public string Billno { get; set; } = null!;

        public string Foliocde { get; set; } = null!;

        public string? Refno2 { get; set; }

        public string? billdesp { get; set; }

        public DateTime Bizdte { get; set; }

        public string Roomno { get; set; } = null!;

        public string Custfullnme { get; set; } = null!;

        public string? Srvccde { get; set; }

        public string? Itemid { get; set; }

        public string? Itemdesc { get; set; }

        public string? uomcde { get; set; }

        public decimal? uomrate { get; set; }

        public decimal? qty { get; set; }

        public decimal pricebill { get; set; }

        public decimal billdiscamt { get; set; }

        public string Currcde { get; set; } = null!;

        public decimal currrate { get; set; }

        public decimal? taxamt { get; set; }

        public string? chrgacccde { get; set; }

        public string? Posid { get; set; }

        public string? Deptcde { get; set; }

        public short shiftno { get; set; }

        public bool? Voidflg { get; set; }

        public short? Voiduserid { get; set; }

        public DateTime? Voiddatetime { get; set; }

        public string? Remark { get; set; }

        public short userid { get; set; }

        public short cmpyid { get; set; }

        public DateTime Revdtetime { get; set; }
    }

    public class InHouseGuestReportModel
    {
        public string guestfullnme { get; set; } = string.Empty;

        public string idppno { get; set; } = string.Empty;

        public string guestlastnme { get; set; } = string.Empty;

        public string phone1 { get; set; } = string.Empty;

        public string phone2 { get; set; } = string.Empty;

        public string emailaddr { get; set; } = string.Empty;

        public string remark { get; set; } = string.Empty;

        public string chrgacccde { get; set; } = string.Empty;


    }

    public class ExpectedCheckInReportModel
    {
        public string resvno { get; set; }

        public string arrivedte { get; set; } = string.Empty;

        public string contactnme { get; set; } = string.Empty;

        public string contactno { get; set; } = string.Empty;

        public string resvmadeby { get; set; } = string.Empty;

        public string confirmcancelby { get; set; } = string.Empty;

        public int nightqty { get; set; }

        public int adult { get; set; }

        public string pickuploc { get; set; } = string.Empty;

        public string specialremark { get; set; } = string.Empty;

        public DateTime confirmdtetime { get; set; }

        public DateTime canceldtetime { get; set; }

        public string checkinid { get; set; } = string.Empty;

        public string userid { get; set; } = string.Empty;

        public DateTime checkInDate { get; set; }
    }

    public class ExpectedCheckOutReportModel
    {
        public string arrivedte { get; set; } = string.Empty;

        public string contactnme { get; set; } = string.Empty;

        public string contactno { get; set; } = string.Empty;

        public string resvmadeby { get; set; } = string.Empty;

        public string confirmcancelby { get; set; } = string.Empty;

        public string confirmdtetime { get; set; } = string.Empty;

        public string specialremark { get; set; } = string.Empty;

        public string userid { get; set; } = string.Empty;

        public DateTime CheckOutDate { get; set; } // for use purpose only

        public DateTime? ActualCheckOutDate { get; set; }// for use purpose only

        public DateTime? ExpectedCheckOutDate { get; set; } // for use purpose only

        public DateTime OccuDate { get; set; } // for use purpose only

        public string CheckInId { get; set; } = string.Empty; // for use purpose only

    }

    public class NoShowCancelReportModel
    {
        public string contactnme { get; set; } = string.Empty;
        public string contactno { get; set; } = string.Empty;
        public string resvmadeby { get; set; } = string.Empty;
        public int nightqty { get; set; }
        public string specialremark { get; set; } = string.Empty;
        public string canceldtetime { get; set; } = string.Empty;
        public string userid { get; set; } = string.Empty;
        public string ResvState { get; set; } = string.Empty; // for use purpose only
        public string LedgerState { get; set; } = string.Empty; // for use purpose only
        public DateTime ResvDate { get; set; } // for use purpose only
        public DateTime OccuDte { get; set; }// for use purpose only

    }

    public class ExtendStaysReportModel
    {
        public string arrivedte { get; set; } = string.Empty;
        public string contactnme { get; set; } = string.Empty;
        public string resvmadeby { get; set; } = string.Empty;
        public string confirmcancelby { get; set; } = string.Empty;
        public string specialremark { get; set; } = string.Empty;
        public string confirmdtetime { get; set; } = string.Empty;
        public string canceldtetime { get; set; } = string.Empty;
        public string userid { get; set; } = string.Empty;
        public string CheckInId { get; set; } = string.Empty; // for use purpose only
        public DateTime ExpectedOutDate { get; set; } // for use purpose only

    }

}
