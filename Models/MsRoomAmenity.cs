namespace Hotel_Core_MVC_V1.Models;

public partial class MsRoomAmenity
{
    public int Rmamtyid { get; set; }

    public string Rmamtycde { get; set; } = null!;

    public string? Rmamtydesc { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
