using System.ComponentModel.DataAnnotations;

namespace Hotel_Core_MVC_V1.Models
{
    public class VisitModel
    {
        public int GuestId { get; set; }

        public string CheckInId { get; set; }

        public int No { get; set; }

        public string GuestName { get; set; }

        public DateTime ArriveDate { get; set; }

        public DateTime DepartDate { get; set; }

        public int NightQty { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal SpendingAmount { get; set; }
    }
}
