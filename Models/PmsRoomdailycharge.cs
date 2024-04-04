namespace Hotel_Core_MVC_V1.Models;

public partial class PmsRoomdailycharge
{
    public int Dailychrgid { get; set; }

    public string Checkinid { get; set; } = null!;

    public int Foliohid { get; set; }

    public DateTime Occudte { get; set; }

    public string Srvccde { get; set; } = null!;

    public decimal Amount { get; set; }

    public short Qty { get; set; }

    public short Cmpyid { get; set; }

    public short Userid { get; set; }

    public DateTime Revdtetime { get; set; }
}
