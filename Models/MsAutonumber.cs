namespace Hotel_Core_MVC_V1.Models;

public partial class MsAutonumber
{
    public short Autonoid { get; set; }

    public string? Billtypcde { get; set; }

    public string? Billprefix { get; set; }

    public bool? Zeroleading { get; set; }

    public short? Runningno { get; set; }

    public long? Lastusedno { get; set; }

    public DateTime? Lastgeneratedte { get; set; }

    public string? Billnature { get; set; }

    public string? Resetevery { get; set; }

    public short? Cmpyid { get; set; }

    public string? Posid { get; set; }

    public string? Posdefloc { get; set; }

    public DateTime? Bizdte { get; set; }

    public byte? Noofshift { get; set; }

    public byte? Curshift { get; set; }

    public string? Posdefsaletyp { get; set; }

    public bool? Allowaccessroom { get; set; }
}
