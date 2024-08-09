namespace Hotel_Core_MVC_V1.Models
{
    public class NightAuditFormModel
    {
        public DateTime NightDate { get; set; }

        public int ShiftNo { get; set; }

        public string LastNightAuditDateTime { get; set; } = string.Empty;

        public bool ButtonFlag { get; set; }

        public string TimeLeft { get; set; } = string.Empty;
        public string StringNightDate { get; set; } = string.Empty;
    }
}
