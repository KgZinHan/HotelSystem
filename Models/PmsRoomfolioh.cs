namespace Hotel_Core_MVC_V1.Models;

public partial class PmsRoomfolioh
{
    public int Foliohid { get; set; }

    public string Foliocde { get; set; } = null!;

    public string Foliodesc { get; set; } = null!;

    public string Checkinid { get; set; } = null!;

    public bool Foliocloseflg { get; set; }

    public short Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
