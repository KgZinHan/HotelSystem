namespace Hotel_Core_MVC_V1.Models;

public partial class MsGuestnationality
{
    public int Nationid { get; set; }

    public string Nationdesc { get; set; } = null!;

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
