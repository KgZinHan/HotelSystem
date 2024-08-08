using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Core_MVC_V1.Models
{
    public class CheckInModels // Sample
    {

    }

    [Keyless]
    public class CheckInModel // To show all type of Check in [expected check-in,today check-in,in-house guest,transfer/upgrade]
    {
        public long RoomLgId { get; set; }

        [DisplayName("CheckIn ID")] public string? CheckInId { get; set; }
        public int No { get; set; }

        [DisplayName("Reservation No.")] public string? Resvno { get; set; } = null!;

        [DisplayName("Arrive Date")] public DateTime Occudte { get; set; }

        [DisplayName("Night Stay")] public short Nightqty { get; set; }

        [DisplayName("Departure Date.")] public DateTime Departdte { get; set; }

        public int RmtypId { get; set; }

        [DisplayName("Room Type")] public string Rmtypdesc { get; set; } = string.Empty;

        public int Rmrateid { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")][DisplayName("Room Price")] public decimal Rmprice { get; set; }

        [DisplayName("Pax No.")] public int PaxNo { get; set; }

        [DisplayName("Adult Qty")] public byte Adultqty { get; set; }

        [DisplayName("Child Qty")] public byte Childqty { get; set; }

        public bool HKeepingFlg { get; set; } = false;

        [DisplayName("Remark")] public string? Remark { get; set; } = null!;

        [DisplayName("Special Instruction")] public string? SpecialInstruct { get; set; }

        [DisplayName("Extra Bed Qty")] public short Extrabedqty { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")][DisplayName("Extra Bed Price")] public decimal Extrabedprice { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")][DisplayName("Disc Amount")] public decimal Discountamt { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")][DisplayName("Amount")] public decimal Amt { get; set; }

        [DisplayName("Payment Type")] public string? Paymenttyp { get; set; }

        [DisplayName("Room No.")] public string? Roomno { get; set; }

        [DisplayName("Contact Name")] public string? ContactName { get; set; } = string.Empty;

        [DisplayName("Contact Ph No.")] public string? ContactNo { get; set; } = string.Empty;

        [DisplayName("Guest Name")] public string GuestName { get; set; } = string.Empty;

        public short? BatchNo { get; set; }

        public int GuestId { get; set; }

        public bool CheckOutFlag { get; set; }

        public string? PreferRoomNo { get; set; }
        public string? StringArriveDte { get; set; }
        public string? StringDepartDte { get; set; }


    }

    public class AvailableRoomNumber // for available room [expected check-in,in-house guest,tranfer/upgrade]
    {
        public int No { get; set; }

        public int? RoomId { get; set; }

        public string RoomNo { get; set; } = string.Empty;

        public int? LocId { get; set; }

        public string? Location { get; set; } = null!;

        public int? BedId { get; set; }

        public string? BedCde { get; set; } = string.Empty;

        public int PaxNo { get; set; }

        public int RmTypId { get; set; }

        public string? RoomType { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal RoomPrice { get; set; }

        public long RoomLgId { get; set; }
    }

    public class GuestInfo // for guest form [expected check-in,in-house guest]
    {
        public int Guestid { get; set; }

        public short? Saluteid { get; set; }

        public string? Salutation { get; set; }

        public string Guestfullnme { get; set; } = null!;

        public string? Guestlastnme { get; set; } = null!;

        public string Idppno { get; set; } = null!;

        public DateTime? Idissuedte { get; set; }

        public byte? Gender { get; set; }

        public string? GenderString { get; set; }

        public DateTime? Dob { get; set; }

        public int? Stateid { get; set; }

        public string? State { get; set; }

        public int? Countryid { get; set; }

        public string? Country { get; set; }

        public int? Nationid { get; set; }

        public string? Nationality { get; set; }

        public bool Vipflg { get; set; }

        public string? Emailaddr { get; set; }

        public string? Phone1 { get; set; }

        public string? Phone2 { get; set; }

        public decimal? Crlimitamt { get; set; }

        public string? Remark { get; set; }

        public short Cmpyid { get; set; }

        public DateTime Revdtetime { get; set; }

        public DateTime? LastVistedDate { get; set; }

        public int? VisitCount { get; set; }

        public int? No { get; set; }

        public bool PrincipleFlg { get; set; }

        public string? ChrgAccCde { get; set; } = string.Empty;
    }

    public class GuestChoose // for guest choose modal [expected check-in,in-house guest]
    {
        public IEnumerable<GuestInfo> GuestInfos { get; set; } = null!;

        public MsGuestdatum Guest { get; set; } = new MsGuestdatum();

        public long? RoomLgId { get; set; }

    }

    public class InHouseGuestModels // for in-house guest edit [in-house guest]
    {
        public CheckInModel CheckInModel { get; set; } = new CheckInModel();

        public IEnumerable<GuestInfo>? CheckInGuestList { get; set; }

    }

}
