namespace Hotel_Core_MVC_V1.Models;

public partial class PmsReservation
{
    public string Resvno { get; set; } = null!;

    public DateTime Resvdtetime { get; set; }

    public DateTime Arrivedte { get; set; }

    public short Nightqty { get; set; }

    public byte Adult { get; set; }

    public byte Child { get; set; }

    public string Resvmadeby { get; set; } = null!;

    public int? Agencyid { get; set; }

    public int? Guestid { get; set; }

    public string Contactnme { get; set; } = null!;

    public string Contactno { get; set; } = null!;

    public string Resvstate { get; set; } = null!;

    public string? Confirmcancelby { get; set; }

    public DateTime? Confirmdtetime { get; set; }

    public DateTime? Canceldtetime { get; set; }

    public bool? Reqpickup { get; set; }

    public string? Pickuploc { get; set; }

    public DateTime? Pickupdtetime { get; set; }

    public string? Specialremark { get; set; }

    public bool Vipstatus { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Cmpyid { get; set; }

    public short Userid { get; set; }
}
