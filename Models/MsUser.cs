namespace Hotel_Core_MVC_V1.Models;

public partial class MsUser
{
    public int Userid { get; set; }

    public string Usercde { get; set; } = null!;

    public string Usernme { get; set; } = null!;

    public byte[]? Pwd { get; set; }

    public string? Deptcde { get; set; }

    public short? Mnugrpid { get; set; }

    public DateTime Revdtetime { get; set; }

    public short Cmpyid { get; set; }
}
