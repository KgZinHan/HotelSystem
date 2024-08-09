using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Core_MVC_V1.Models
{
    public class HotelModels
    {
    }

    public class HotelInfoModel
    {
        public int cmpyid { get; set; }
        [DisplayName("Hotel")] public string? hotelnme { get; set; }
        [DisplayName("Area")] public int areaid { get; set; }
        public string? area { get; set; }
        [DisplayName("Township")] public int tspid { get; set; }
        public string? tsp { get; set; }
        [DisplayName("Address")] public string? address { get; set; }
        [DisplayName("Hotel Date")] public string hoteldte { get; set; } = string.Empty;
        [DisplayName("Phone & Website")] public string? phone1 { get; set; }
        public string? phone2 { get; set; }
        public string? phone3 { get; set; }
        [DisplayName("Email")] public string? email { get; set; }
        [DisplayName("Website")] public string? website { get; set; }
        [DisplayName("Check-In/Out")] public string? checkintime { get; set; }
        [DisplayName("Check Out Time")] public TimeSpan? checkouttime { get; set; }
        [DisplayName("Late Check-In")] public string? latecheckintime { get; set; }
        [DisplayName("Auto Post?")] public bool autopostflg { get; set; }
        [DisplayName("Auto Post Time")] public string? autoposttime { get; set; }
        [DisplayName("Revision Datetime")]public DateTime revdtetime { get; set; }
        public int userid { get; set; }
        [DisplayName("Revised by")] public string UserCode { get; set; } = string.Empty;
    }

    [Keyless]
    public class HotelRoomModel
    {
        public int Roomid { get; set; }
        [DisplayName("Room No")]
        public string Roomno { get; set; } = null!;
        [DisplayName("Location")]

        public int Locid { get; set; }
        [DisplayName("Location")]
        public string? Loccde { get; set; }
        [DisplayName("Room Type")]

        public int? Rmtypid { get; set; }
        public string? rmtypcde { get; set; }
        [DisplayName("Bedding")]
        public int? Bedid { get; set; }

        public string? bedcde { get; set; }
        [DisplayName("Room Amenities")]

        public List<int>? Rmamtyid { get; set; }
        public string? RoomAmenities { get; set; } = null!;
        [DisplayName("Room feature")]

        public List<int>? Rmfeatureid { get; set; }
        public string? RoomFeatures { get; set; }
        [DisplayName("Auto Apply Rate?")]
        public bool Isautoapplyrate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [DisplayName("Use Fix Price")]
        public decimal? Usefixprice { get; set; }

        public int Paxno { get; set; }
        [DisplayName("Room Tel. No.")]
        public string? Roomtelno { get; set; }
        [DisplayName("Guest In?")]
        public bool Isguestin { get; set; }
        [DisplayName("Guest Active Message")]
        public string? Guestactivemsg { get; set; }
        [DisplayName("Dnd?")]
        public bool Isdnd { get; set; }

        public int Cmpyid { get; set; }

        public DateTime Revdtetime { get; set; }

        public int Userid { get; set; }

    }

    public class AreaModel
    {
        public int areaid { get; set; }
        public string? areacde { get; set; }
        public string? areadesc { get; set; }
    }

    public class TownshipModel
    {
        public int tspid { get; set; }
        public string? tspcde { get; set; }
        public string? tspcdesc { get; set; }
        public int areaid { get; set; }
    }

    public class HotelRoomRateModel
    {
        public int Rmrateid { get; set; }

        public string Rmratecde { get; set; } = null!;

        public string? Rmratedesc { get; set; }

        public DateTime? Daymonthstart { get; set; }

        public DateTime? Daymonthend { get; set; }

        public int? Rmtypid { get; set; }
        public string Rmtypcde { get; set; } = null!;
        public decimal Price { get; set; }

        public int Cmpyid { get; set; }

        public DateTime Revdtetime { get; set; }

        public int Userid { get; set; }
    }

    public class UserModel
    {
        public int Userid { get; set; }
        [DisplayName("User Code")] public required string Usercde { get; set; }
        [DisplayName("User Name")] public required string Usernme { get; set; }
        [DisplayName("Password")] public string? Pwd { get; set; }
        [DisplayName("Confirm Password")] public string? ConfirmPassword { get; set; }
        [DisplayName("New Password")] public string NewPassword { get; set; } = string.Empty;
        [DisplayName("Menu Group")] public short? Mnugrpid { get; set; }
        [DisplayName("Department")] public string? Deptcde { get; set; }
        public DateTime Revdtetime { get; set; }
        [DisplayName("Hotel Name")] public short Cmpyid { get; set; }
        [DisplayName("Hotel Name")] public string? Cmpy { get; set; }
    }

    public class LoginModel
    {
        public string? Usernme { get; set; }
        public string Usercde { get; set; } = null!;
        public string Pwd { get; set; } = null!;
        public short Cmpyid { get; set; }
    }

    public class ChangePasswordModel
    {
        public string Oldpwd { get; set; } = null!;
        public string Newpwd { get; set; } = null!;
        public string Confirmpwd { get; set; } = null!;
    }

    public class GuestStateModel
    {
        public int Gstateid { get; set; }

        [DisplayName("State Description")] public string Gstatedesc { get; set; } = null!;

        [DisplayName("Country")] public string Country { get; set; }

        public DateTime Revdtetime { get; set; }

        public short Userid { get; set; }
    }

    public class HotelRoomTypeModel
    {
        public int Rmtypid { get; set; }

        public string Rmtypcde { get; set; } = null!;

        public string? Rmtypdesc { get; set; }

        public int Paxno { get; set; }

        public decimal Extrabedprice { get; set; }

        public int Cmpyid { get; set; }

        public DateTime Revdtetime { get; set; }

        public int Userid { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] 
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int ExtraBedQty { get; set; }
        public int RoomRateId { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public virtual ICollection<MsHotelRoomTypeImage> MsHotelRoomTypeImages { get; set; } = new List<MsHotelRoomTypeImage>();

        public IFormFile? RmTypMainImg { get; set; }

        public List<IFormFile>? RmTypImgList { get; set; } = null!;

        public string? Base64Image { get; set; } = null!;//edit

        public List<string>? Base64ImageList { get; set; } = null!;//edit
    }

    public class UserInfoModel
    {
        public string[][] Data { get; set; }
        public string ContactNme { get; set; } = null!;
        public string ContactNo { get; set; } = null!;
        public string? SpecialRemark { get; set; }
        public bool VipStatus { get; set; } = false;
    }


}
