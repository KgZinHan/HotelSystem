namespace Hotel_Core_MVC_V1.Models;

public partial class MsMessageeditor
{
    public int Msgtypid { get; set; }

    public string Msgtypcde { get; set; } = null!;

    public string? Msgtodept { get; set; }

    public string? Msgtoperson { get; set; }

    public string Msgdetail { get; set; } = null!;

    public int Guestid { get; set; }

    public string? Checkinid { get; set; }

    public string? Raisebynme { get; set; }

    public string Priority { get; set; } = null!;

    public bool Resolveflg { get; set; }

    public string? Resolvedetail { get; set; }

    public short Userid { get; set; }

    public DateTime Takedtetime { get; set; }

    public DateTime? Resolvedtetime { get; set; }
}
