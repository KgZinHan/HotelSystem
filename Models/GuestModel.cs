namespace Hotel_Core_MVC_V1.Models
{
    public class GuestModel
    {
    }

    public class GuestDBModel
    {
        public int guestId { get; set; }

        public string guestfullnme { get; set; }

        public string idppno { get; set; }

        public string? chrgacccde { get; set; }

        public DateTime? lastvisitdte { get; set; }

        public int? visitcount { get; set; } //test
    }
}
