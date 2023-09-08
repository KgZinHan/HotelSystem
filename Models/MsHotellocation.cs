using System;
using System.Collections.Generic;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotellocation
{
    public int Locid { get; set; }

    public string Loccde { get; set; } = null!;

    public string? Locdesc { get; set; }

    public int Cmpyid { get; set; }

    public bool Isoutlet { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
