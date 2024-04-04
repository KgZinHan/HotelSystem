namespace Hotel_Core_MVC_V1.Models;

public partial class MsReason
{
    public short Reasonid { get; set; }

    public string Reasondesc { get; set; } = null!;

    public bool Guestuseflg { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
