namespace Hotel_Core_MVC_V1.Models;

public partial class MsCurrency
{
    public int Currid { get; set; }

    public string Currtyp { get; set; } = null!;

    public string? Currcde { get; set; }

    public decimal? Currrate { get; set; }

    public bool? Homeflg { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
