namespace Hotel_Core_MVC_V1.Models
{
    public class RoomEnquiry
    {

    }
    #region // Room Enquiry

    public class RoomEnquiryDB
    {
        public DateTime ThisDate { get; set; }

        public string RoomType { get; set; } = string.Empty;

        public int RoomQty { get; set; }

        public int BookQty { get; set; }

        public string? OccuState { get; set; }

    }

    public class RoomEnquiryInfo
    {
        public string RoomType { get; set; } = string.Empty;

        public string RmTypDesc { get; set; } = string.Empty;

        public int Info1 { get; set; }

        public int Info2 { get; set; }

        public int Info3 { get; set; }

        public int Info4 { get; set; }

        public int Info5 { get; set; }

        public int Info6 { get; set; }

        public int Info7 { get; set; }

        public int Info8 { get; set; }

        public int Info9 { get; set; }

        public int Info10 { get; set; }

        public int Info11 { get; set; }

        public int Info12 { get; set; }

        public int Info13 { get; set; }

        public int Info14 { get; set; }

        public int Info15 { get; set; }

        public int Info16 { get; set; }

        public int Info17 { get; set; }

        public int Info18 { get; set; }

        public int Info19 { get; set; }

        public int Info20 { get; set; }

        public int Info21 { get; set; }

        public int Info22 { get; set; }

        public int Info23 { get; set; }

        public int Info24 { get; set; }

        public int Info25 { get; set; }

        public int Info26 { get; set; }

        public int Info27 { get; set; }

        public int Info28 { get; set; }

        public int Info29 { get; set; }

        public int Info30 { get; set; }

    }

    public class RoomEnquiryModels
    {
        public IEnumerable<DateTime> DateList { get; set; } = new List<DateTime>();

        public IEnumerable<RoomEnquiryInfo> RoomInfoList { get; set; } = new List<RoomEnquiryInfo>();

        public int NightCount { get; set; }

        public int TotalRoom { get; set; }

        public int Reserved { get; set; }

        public int Occupied { get; set; }

        public int Maintenance { get; set; }

        public float Occupancy { get; set; }
    }

    #endregion


    #region // Room No. Enquiry

    public class RoomNoEnquiryDB
    {
        public DateTime ThisDate { get; set; }

        public string RoomNo { get; set; } = string.Empty;

        public int RmTypId { get; set; }

        public string RoomType { get; set; } = string.Empty;

        public DateTime? OccuDate { get; set; }

        public string? GuestFullName { get; set; }

    }

    public class RoomNoEnquiryInfo
    {
        public string RoomNo { get; set; } = string.Empty;

        public string Info1 { get; set; }

        public string Info2 { get; set; }

        public string Info3 { get; set; }

        public string Info4 { get; set; }

        public string Info5 { get; set; }

        public string Info6 { get; set; }

        public string Info7 { get; set; }

    }

    public class RoomNoEnquiryModels
    {
        public IEnumerable<DateTime> DateList { get; set; } = new List<DateTime>();

        public IEnumerable<RoomNoEnquiryInfo> RoomNoInfoList { get; set; } = new List<RoomNoEnquiryInfo>();

        public string RoomType { get; set; }

        public int NightCount { get; set; }

        public int Day1 { get; set; }

        public int Day2 { get; set; }

        public int Day3 { get; set; }

        public int Day4 { get; set; }

        public int Day5 { get; set; }

        public int Day6 { get; set; }

        public int Day7 { get; set; }

    }

    #endregion

}
