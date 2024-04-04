namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelservicegrpd
{
    public string Srvccde { get; set; } = null!;

    public string? Srvcdesc { get; set; }

    public short Cmpyid { get; set; }

    public string? Deptcde { get; set; }

    public string? Srvcgrpcde { get; set; }

    public string Trantyp { get; set; } = null!;

    public decimal Srvcamt { get; set; }

    public string? Srvcaccde { get; set; }

    public bool Sysdefine { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
