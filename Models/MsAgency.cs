namespace Hotel_Core_MVC_V1.Models;

public partial class MsAgency
{
    public int Agencyid { get; set; }

    public short Cmpyid { get; set; }

    public string Agencynme { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Contperson { get; set; }

    public string? Address { get; set; }

    public string? Website { get; set; }

    public decimal Creditlimit { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
