namespace Hotel_Core_MVC_V1.Models
{
    public class AvailableRoomTypeModel
    {
        public int CmpyID { get; set; }
        public string rmtypdesc { get; set; } = string.Empty;
        public int RoomType { get; set; }
        public int RateID { get; set; }
        public decimal Price { get; set; }
        public int AvilQty { get; set; }
    }

    public class ReservationJSList
    {
        public List<List<string>> HotelRoomInfos { get; set; } = null!;
        public Dictionary<string, string> UserInfos { get; set; } = null!;
    }
}
