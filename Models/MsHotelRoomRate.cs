namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelRoomRate
{
    public int Rmrateid { get; set; }

    public string Rmratecde { get; set; } = null!;

    public string? Rmratedesc { get; set; }

    public DateTime? Daymonthstart { get; set; }

    public DateTime? Daymonthend { get; set; }

    public int Rmtypid { get; set; }

    public decimal Price { get; set; }

    public decimal? Execrate { get; set; }

    public bool? Defrateflg { get; set; }

    public int Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
