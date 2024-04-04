namespace Hotel_Core_MVC_V1.Models
{
    public class AvailableRoomNumberModel
    {
        public int? roomid { get; set; }
        public string roomno { get; set; }
        public int rmtypid { get; set; }
        public int bedid { get; set; }

        public int? locid { get; set; }
    }

    public class RoomEnquiryDBModel
    {
        public DateTime theDate { get; set; }

        public string rmtypcde { get; set; }

        public int roomqty { get; set; }

        public int bookqty { get; set; }

        public string? occustate { get; set; }

    }

    public class RoomNoEnquiryDBModel
    {
        public DateTime theDate { get; set; }

        public string roomno { get; set; }

        public int rmtypid { get; set; }

        public string rmtypcde { get; set; }

        public DateTime? occudte { get; set; }

        public string? guestfullnme { get; set; }

    }

}
