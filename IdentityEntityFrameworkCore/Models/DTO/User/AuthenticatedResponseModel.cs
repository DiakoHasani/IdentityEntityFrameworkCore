namespace IdentityEntityFrameworkCore.Models.DTO.User
{
    public class AuthenticatedResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
