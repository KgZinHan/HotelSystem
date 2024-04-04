namespace Hotel_Core_MVC_V1.Models;

public partial class PmsCheckinroomguest
{
    public int Rmguestid { get; set; }

    public string Checkinid { get; set; } = null!;

    public int Guestid { get; set; }

    public bool Principleflg { get; set; }
}
