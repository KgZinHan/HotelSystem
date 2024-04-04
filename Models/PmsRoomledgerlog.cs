namespace Hotel_Core_MVC_V1.Models;

public partial class PmsRoomledgerlog
{
    public long Roomlogid { get; set; }

    public string? Resvno { get; set; }

    public short Cmpyid { get; set; }

    public string? Checkinid { get; set; }

    public DateTime? Occudte { get; set; }

    public string? Occustate { get; set; }

    public bool Hkeepingflg { get; set; }

    public int Rmtypid { get; set; }

    public string? Roomno { get; set; }

    public int Rmrateid { get; set; }

    public decimal Price { get; set; }

    public short Extrabedqty { get; set; }

    public decimal Extrabedprice { get; set; }

    public decimal Discountamt { get; set; }

    public string? Preferroomno { get; set; }

    public string? Occuremark { get; set; }

    public short? Batchno { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
