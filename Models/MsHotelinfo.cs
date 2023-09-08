using System;
using System.Collections.Generic;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelinfo
{
    public short Cmpyid { get; set; }

    public string Hotelnme { get; set; } = null!;

    public int Areaid { get; set; }

    public int Tspid { get; set; }

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

    public bool Autopostflg { get; set; }

    public TimeSpan? Autoposttime { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
