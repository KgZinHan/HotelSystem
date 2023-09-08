using System;
using System.Collections.Generic;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsTownship
{
    public int Tspid { get; set; }

    public string Tspcde { get; set; } = null!;

    public string Tspdesc { get; set; } = null!;

    public int Areaid { get; set; }
}
