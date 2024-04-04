namespace Hotel_Core_MVC_V1.Models;

public partial class MsGuestdatum
{
    public int Guestid { get; set; }

    public short? Saluteid { get; set; }

    public string Guestfullnme { get; set; } = null!;

    public string? Guestlastnme { get; set; }

    public string Idppno { get; set; } = null!;

    public DateTime? Idissuedte { get; set; }

    public byte? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public int? Stateid { get; set; }

    public int? Countryid { get; set; }

    public int? Nationid { get; set; }

    public bool Vipflg { get; set; }

    public string? Emailaddr { get; set; }

    public string? Phone1 { get; set; }

    public string? Phone2 { get; set; }

    public decimal? Crlimitamt { get; set; }

    public string? Remark { get; set; }

    public string? Chrgacccde { get; set; }

    public short Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }

    public DateTime? Lastvisitdte { get; set; }

    public int? Visitcount { get; set; }
}
