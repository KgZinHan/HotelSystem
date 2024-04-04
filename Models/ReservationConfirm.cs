
namespace Hotel_Core_MVC_V1.Models
{
    public class SearchReservation // To Search 
    {
        public string GuestName { get; set; } = null!;
        public string ReserveState { get; set; } = null!;
        public bool Active { get; set; }
        public DateTime FromDate { get; set; } = new DateTime(1990, 1, 1);

        public DateTime ToDate { get; set; } = new DateTime(1990, 1, 1);
    }

    public class ReservationConfirm // To Show Table
    {
        public int No { get; set; }

        public string ResvNo { get; set; } = null!;

        public DateTime ResvDate { get; set; }

        public string GuestName { get; set; } = null!;

        public DateTime ArriveDate { get; set; }

        public string ContactNo { get; set; } = null!;

        public DateTime DepartDate { get; set; }

        public int RoomQty { get; set; }

        public string Agency { get; set; } = null!;

        public string State { get; set; } = null!;

        public string Remark { get; set; } = null!;
    }

    public class ReserveRoomDetails // To Show Details
    {
        public int Rmtypid { get; set; }

        public string Rmtypdesc { get; set; } = null!;

        public decimal Price { get; set; }

        public short Extrabedqty { get; set; }

        public decimal Extrabedprice { get; set; }
    }

    public class ReservationActivity // For Reservation Activity
    {
        public int ReserveCount { get; set; }
        public int ConfirmCount { get; set; }
        public int CancelCount { get; set; }
    }

    public class ReservationConfirmList
    {
        public IEnumerable<ReservationConfirm> ResvConfirms { get; set; }

        public SearchReservation SearchResv { get; set; }

        public ReservationActivity ResvAct { get; set; }
    }
}
