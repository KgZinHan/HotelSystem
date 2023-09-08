using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel;

namespace Hotel_Core_MVC_V1.Models
{
    public class HotelModels
    {
    }
    public class HotelInfoModel
    {
        public int cmpyid { get; set; }
        [DisplayName("Hotel")]
        public string? hotelnme { get; set; }
        [DisplayName("Area")]
        public int areaid { get; set; }
        public string? area {  get; set; }
        [DisplayName("Township")]
        public int tspid { get; set; }
        public string? tsp { get; set; }
        [DisplayName("Address")]
        public string? address { get; set; }
        public DateTime? hoteldte { get; set; }
        [DisplayName("Phone & Website")]
        public string? phone1 { get; set; }
        public string? phone2 { get; set; }
        public string? phone3 { get; set; }
        [DisplayName("Email")]
        public string? email { get; set; }
        [DisplayName("Website")]
        public string? website { get; set; }
        [DisplayName("Check In/Out")]
        public string? checkintime { get; set; }
        public TimeSpan? checkouttime { get; set; }
        [DisplayName("Late Checkin")]
        public string? latecheckintime { get; set; }
        [DisplayName("Auto Post?")]
        public bool autopostflg { get; set; }
        [DisplayName("Auto Post Time")]
        public string? autoposttime { get; set; }
        public DateTime revdtetime { get; set; }
        public int userid { get; set; }
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
        public string? RoomAmenities { get; set; }=null!;
        [DisplayName("Room feature")]

        public List<int>? Rmfeatureid { get; set; }
        public string? RoomFeatures { get; set; }
        [DisplayName("Auto Apply Rate?")]
        public bool Isautoapplyrate { get; set; }
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
        [DisplayName("User Code")]

        public string Usercde { get; set; } = null!;
        [DisplayName("User Name")]

        public string Usernme { get; set; } = null!;
        [DisplayName("Password")]

        public string? Pwd { get; set; }
        [DisplayName("Menu Group")]

        public short? Mnugrpid { get; set; }
        public DateTime Revdtetime { get; set; }
        [DisplayName("Hotel Name")]

        public short Cmpyid { get; set; }
        public string? Cmpy { get; set; }
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
    //public class HotelLocationModel
    //{
    //    public int Locid { get; set; }

    //    public string Loccde { get; set; } = null!;

    //    public string? Locdesc { get; set; }

    //    public int Cmpyid { get; set; }

    //    public bool Isoutlet { get; set; }

    //    public DateTime Revdtetime { get; set; }

    //    public int Userid { get; set; }
    //}
    //public class HotelRoomBeddingModel
    //{
    //    public int bedid { get; set; }
    //    public string? bedcde { get; set; }
    //    public string? beddesc { get;set; }
    //    public int cmpyid { get; set; }
    //    public DateTime revdtetime { get; set; }
    //    public int userid { get; set; }
    //}
    //public class HotelRoomStateModel
    //{
    //    public int Rmstateid { get; set; }

    //    public string Rmstatecde { get; set; } = null!;

    //    public string? Rmstatedesc { get; set; }

    //    public int? Syscolor { get; set; }

    //    public int Cmpyid { get; set; }

    //    public DateTime Revdtetime { get; set; }

    //    public int Userid { get; set; }
    //}
}
