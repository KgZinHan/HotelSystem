namespace Hotel_Core_MVC_V1.Models;

public partial class MsGuestcountry
{
    public int Countryid { get; set; }

    public string Countrydesc { get; set; } = null!;

    public int Defstateid { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
