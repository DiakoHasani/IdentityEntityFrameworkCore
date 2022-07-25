namespace IdentityEntityFrameworkCore.Models.DTO.User
{
    public class UpdateRefreshToken
    {
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string UserId { get; set; }
    }
}
