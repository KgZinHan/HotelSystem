namespace Hotel_Core_MVC_V1.Models;

public partial class MsRoomFeature
{
    public int Rmfeatureid { get; set; }

    public string Rmfeaturecde { get; set; } = null!;

    public string? Rmfeaturedesc { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
