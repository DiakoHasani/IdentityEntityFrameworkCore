using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityEntityFrameworkCore.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(length: 40)]
        public string Name { get; set; } = "";

        [MaxLength(length: 40)]
        public string Family { get; set; } = "";

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
