namespace IdentityEntityFrameworkCore.Models.DTO.User
{
    public class ResultRegisterModel
    {
        public bool Result { get; set; } = false;
        public List<string> Message { get; set; } = new List<string>();
        public string UserId { get; set; }
    }
}
