using System;
using System.Collections.Generic;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsArea
{
    public int Areaid { get; set; }

    public string Areacde { get; set; } = null!;

    public string? Areadesc { get; set; }
}
