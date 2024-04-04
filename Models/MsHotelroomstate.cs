namespace Hotel_Core_MVC_V1.Models;

public partial class MsHotelroomstate
{
    public int Rmstateid { get; set; }

    public string Rmstatecde { get; set; } = null!;

    public string? Rmstatedesc { get; set; }

    public int? Syscolor { get; set; }

    public int Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public int Userid { get; set; }
}
