using System.ComponentModel.DataAnnotations;

namespace IdentityEntityFrameworkCore.Models.DTO.User
{
    public class RegisterBody
    {
        [Display(Name = "نام")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        [MaxLength(length: 40, ErrorMessage = "حداکثر تعداد کاراکتر {0} 40 کاراکتر است")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        [MaxLength(length: 40, ErrorMessage = "حداکثر تعداد کاراکتر {0} 40 کاراکتر است")]
        public string LastName { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        [MaxLength(length: 11, ErrorMessage = "حداکثر تعداد کاراکتر {0} 11 کاراکتر است")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        [MaxLength(length: 60, ErrorMessage = "حداکثر تعداد کاراکتر {0} 60 کاراکتر است")]
        public string Email { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        public string Password { get; set; }

        [Display(Name = "تکرار رمز عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} باید پرشود")]
        [Compare(nameof(Password), ErrorMessage = "{0} مطابقت ندارد")]
        public string ComparePassword { get; set; }
    }
}
