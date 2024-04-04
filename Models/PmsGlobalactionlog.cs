namespace Hotel_Core_MVC_V1.Models;

public partial class PmsGlobalactionlog
{
    public int Actionlogid { get; set; }

    public string Formnme { get; set; } = null!;

    public string? Btnaction { get; set; }

    public string? Actiondetail { get; set; }

    public short Userid { get; set; }

    public DateTime Revdtetime { get; set; }
}
