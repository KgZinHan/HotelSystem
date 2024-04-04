namespace Hotel_Core_MVC_V1.Models;

public partial class MsGueststate
{
    public int Gstateid { get; set; }

    public string Gstatedesc { get; set; } = null!;

    public int Countryid { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
