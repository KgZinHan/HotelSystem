﻿using System;
using System.Collections.Generic;

namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelroombedding
{
    public int Bedid { get; set; }

    public string Bedcde { get; set; } = null!;

    public string? Beddesc { get; set; }

    public int Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
