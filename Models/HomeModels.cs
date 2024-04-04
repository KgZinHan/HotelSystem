using System.ComponentModel.DataAnnotations;

namespace Hotel_Core_MVC_V1.Models
{
    public class HomeModels
    {
    }

    public class HomeModel
    {
        public int No { get; set; }

        public long RoomLgId { get; set; }

        public string? CheckInid { get; set; }

        public string? ResvNo { get; set; }

        public string GuestName { get; set; } = string.Empty;

        public string? GuestNo { get; set; }

        public string? RoomNo { get; set; }

        public DateTime CheckInDate { get; set; }

        public int NightQty { get; set; }

        public DateTime? DepartDate { get; set; }

        public int RmTypId { get; set; }

        public string? RoomType { get; set; }

        public string? OccuState { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal FolioBalance { get; set; }

    }

    public class Occupancies

    {
        public string Description { get; set; } = string.Empty;

        public int Status { get; set; }

    }
}
