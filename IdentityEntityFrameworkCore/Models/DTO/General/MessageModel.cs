namespace IdentityEntityFrameworkCore.Models.DTO.General
{
    public class MessageModel
    {
        public int Code { get; set; }
        public bool Result { get; set; } = false;
        public string Message { get; set; } = "";
    }
}
