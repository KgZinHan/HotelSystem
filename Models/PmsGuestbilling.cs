namespace Hotel_Core_MVC_V1.Models;

public partial class PmsGuestbilling
{
    public int Billd { get; set; }

    public string Checkinid { get; set; } = null!;

    public string Billno { get; set; } = null!;

    public string Foliocde { get; set; } = null!;

    public string? Refno2 { get; set; }

    public string? Billdesp { get; set; }

    public DateTime Bizdte { get; set; }

    public string Roomno { get; set; } = null!;

    public string Custfullnme { get; set; } = null!;

    public string? Srvccde { get; set; }

    public string? Itemid { get; set; }

    public string? Itemdesc { get; set; }

    public string? Uomcde { get; set; }

    public decimal? Uomrate { get; set; }

    public decimal? Qty { get; set; }

    public decimal Pricebill { get; set; }

    public decimal Billdiscamt { get; set; }

    public string Currcde { get; set; } = null!;

    public decimal Currrate { get; set; }

    public decimal? Taxamt { get; set; }

    public string? Chrgacccde { get; set; }

    public string? Posid { get; set; }

    public string? Deptcde { get; set; }

    public short Shiftno { get; set; }

    public bool? Voidflg { get; set; }

    public short? Voiduserid { get; set; }

    public DateTime? Voiddatetime { get; set; }

    public string? Remark { get; set; }

    public short Userid { get; set; }

    public short Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }
}
