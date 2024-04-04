namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelRoomTypeImage
{
    public int Rmtypimgid { get; set; }

    public string Rmtypimgdesc { get; set; } = null!;

    public byte[] Rmtypmainimg { get; set; } = null!;

    public bool Mainimgflg { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }

    public int Rmtypid { get; set; }
}
