namespace Hotel_Core_MVC_V1.Models
{
    public class MessageModel
    {

        public int MsgTypId { get; set; }

        public string MsgTypCde { get; set; } = null!;

        public bool ResolvedFlg { get; set; }

        public string? UserName { get; set; }

        public string Msgdetail { get; set; } = null!;

        public string MsgTo { get; set; } = null!;

        public string Takedtetime { get; set; } = null!;

        public string Priority { get; set; } = null!;

        public string Resolved { get; set; } = string.Empty;
    }

    public class MessageModels
    {
        public IEnumerable<MessageModel> Messages { get; set; } = new List<MessageModel>();

        public MsMessageeditor MessagesEditor { get; set; } = new MsMessageeditor();

        public int totalMessage { get; set; } = 0;
    }
}
