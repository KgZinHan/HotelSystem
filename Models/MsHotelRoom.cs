using System;
using System.Collections.Generic;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelRoom
{
    public int Roomid { get; set; }

    public string Roomno { get; set; } = null!;

    public int Locid { get; set; }

    public int? Rmtypid { get; set; }

    public int? Bedid { get; set; }

    public bool Isautoapplyrate { get; set; }

    public decimal? Usefixprice { get; set; }

    public int Paxno { get; set; }

    public string? Roomtelno { get; set; }

    public bool Isguestin { get; set; }

    public string? Guestactivemsg { get; set; }

    public bool Isdnd { get; set; }

    public int Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
