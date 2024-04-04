namespace Hotel_Core_MVC_V1.Models;

public partial class PmsCheckin
{
    public string Checkinid { get; set; } = null!;

    public string? Checkinno { get; set; }

    public short Cmpyid { get; set; }

    public byte Adultqty { get; set; }

    public byte Childqty { get; set; }

    public DateTime Checkindte { get; set; }

    public short Nightqty { get; set; }

    public int? Agencyid { get; set; }

    public string Contactnme { get; set; } = null!;

    public string Contactno { get; set; } = null!;

    public string? Paymenttyp { get; set; }

    public string? Specialinstruct { get; set; }

    public bool Checkoutflg { get; set; }

    public DateTime? Checkoutdtetime { get; set; }

    public DateTime? Checkindtetime { get; set; }

    public string? Remark { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
