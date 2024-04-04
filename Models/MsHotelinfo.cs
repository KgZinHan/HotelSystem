using System.ComponentModel;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelinfo
{
    public short Cmpyid { get; set; }

    [DisplayName("Hotel Name")]public string Hotelnme { get; set; } = null!;

    [DisplayName("Area")] public int Areaid { get; set; }

    [DisplayName("Township")] public int Tspid { get; set; }

    public string Address { get; set; } = null!;

    public DateTime? Hoteldte { get; set; }

    public string? Phone1 { get; set; }

    public string? Phone2 { get; set; }

    public string? Phone3 { get; set; }

    public string Email { get; set; } = null!;

    public string? Website { get; set; }

    public TimeSpan? Checkintime { get; set; }

    public TimeSpan? Checkouttime { get; set; }

    public TimeSpan? Latecheckintime { get; set; }

    public byte? Noofshift { get; set; }

    public byte? Curshift { get; set; }

    public bool Autopostflg { get; set; }

    public TimeSpan? Autoposttime { get; set; }

    public DateTime? Lastrundtetime { get; set; }

    public short? Nightauditintervalhr { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
