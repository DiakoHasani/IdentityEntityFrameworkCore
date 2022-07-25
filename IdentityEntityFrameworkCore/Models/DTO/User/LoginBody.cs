using System.ComponentModel.DataAnnotations;

namespace IdentityEntityFrameworkCore.Models.DTO.User
{
    public class LoginBody
    {
        [Display(Name = "ایمیل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        public string Email { get; set; }

        [Display(Name = "رمزعبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        public string Password { get; set; }
    }
}
