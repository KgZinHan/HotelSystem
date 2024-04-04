namespace Hotel_Core_MVC_V1.Models;

public partial class MsGuestsalutation
{
    public short Saluteid { get; set; }

    public string Salutedesc { get; set; } = null!;

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
