namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelRoomType
{
    public int Rmtypid { get; set; }

    public string Rmtypcde { get; set; } = null!;

    public string? Rmtypdesc { get; set; }

    public int Paxno { get; set; }

    public decimal? Extrabedprice { get; set; }

    public int Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
