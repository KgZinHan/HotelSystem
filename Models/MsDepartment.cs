namespace Hotel_Core_MVC_V1.Models;

public partial class MsDepartment
{
    public string Deptcde { get; set; } = null!;

    public string Deptdesc { get; set; } = null!;

    public short Cmpyid { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Userid { get; set; }
}
