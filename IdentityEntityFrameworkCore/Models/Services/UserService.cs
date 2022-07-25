using IdentityEntityFrameworkCore.Models.Domain;
using IdentityEntityFrameworkCore.Models.DTO.General;
using IdentityEntityFrameworkCore.Models.DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityEntityFrameworkCore.Models.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public UserService(UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ResultRegisterModel> AddUser(RegisterBody model)
        {
            try
            {
                if (CheckExistUserByEmail(model.Email))
                {
                    return new ResultRegisterModel
                    {
                        Message = new List<string> { "این ایمیل قبلا در سامانه ثبت شده است" }
                    };
                }

                if (CheckExistUserByPhoneNumber(model.PhoneNumber))
                {
                    return new ResultRegisterModel
                    {
                        Message = new List<string> { "این شماره موبایل قبلا در سیستم ثبت شده است" }
                    };
                }

                var user = new ApplicationUser
                {
                    Email = model.Email,
                    EmailConfirmed = true,
                    Family = model.LastName,
                    Name = model.FirstName,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    UserName = model.Email
                };

                var resultAdd = await _userManager.CreateAsync(user, model.Password);
                if (resultAdd.Succeeded)
                {
                    return new ResultRegisterModel
                    {
                        Result = true,
                        UserId = user.Id
                    };
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var error in resultAdd.Errors)
                    {
                        if (error.Code.Equals("PasswordRequiresNonAlphanumeric"))
                        {
                            errors.Add("رمز عبور باید حداقل یک کاراکتر غیر الفبایی داشته باشد");
                        }
                        else if (error.Code.Equals("PasswordRequiresLower"))
                        {
                            errors.Add("رمز عبور باید حداقل یک حروف کوچک داشته باشد");
                        }
                        else if (error.Code.Equals("PasswordRequiresUpper"))
                        {
                            errors.Add("رمز عبور باید حداقل یک حروف بزرگ داشته باشد");
                        }
                        else if (error.Code.Equals("PasswordTooShort"))
                        {
                            errors.Add("رمز عبور باید حداقل 8 کاراکتر باشد");
                        }
                        else
                        {
                            errors.Add(error.Description);
                        }
                    }

                    return new ResultRegisterModel { Message = errors };
                }
            }
            catch (Exception)
            {
                return new ResultRegisterModel
                {
                    Message = new List<string> { "خطای رخ داده است" }
                };
            }
        }

        public bool CheckExistUserByEmail(string email)
        {
            return _userManager.Users.Where(a => a.Email.Equals(email)).Any();
        }

        public bool CheckExistUserById(string userId)
        {
            return _userManager.Users.Where(a => a.Id.Equals(userId)).Any();
        }

        public bool CheckExistUserByPhoneNumber(string phoneNumber)
        {
            return _userManager.Users.Where(a => a.Email.Equals(phoneNumber)).Any();
        }

        public bool CheckExistUserByUserName(string userName)
        {
            return _userManager.Users.Where(a => a.UserName.Equals(userName)).Any();
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            return _userManager.Users.Where(a => a.Email.Equals(email)).FirstOrDefault();
        }

        public ApplicationUser GetUserById(string userId)
        {
            return _userManager.Users.Where(a => a.Id.Equals(userId)).FirstOrDefault();
        }

        public ApplicationUser GetUserByUserName(string userName)
        {
            return _userManager.Users.Where(a => a.UserName.Equals(userName)).FirstOrDefault();
        }

        public async Task<ResultLoginModel> Login(LoginBody model)
        {
            try
            {
                var user = GetUserByUserName(model.Email);
                if (user is null)
                {
                    return new ResultLoginModel { Message = new List<string> { "چنین کاربری یافت نشد" } };
                }

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return new ResultLoginModel { Message = new List<string> { "رمز عبور اشتباه است" } };
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles is null || userRoles.Count() == 0)
                {
                    return new ResultLoginModel { Message = new List<string> { "شما هیچ نقشی در سیستم ندارید" } };
                }
                return new ResultLoginModel
                {
                    Result = true,
                    UserId = user.Id,
                    UserRoles = userRoles.ToList()
                };
            }
            catch (Exception)
            {
                return new ResultLoginModel
                {
                    Message = new List<string> { "خطای رخ داده است" }
                };
            }
        }

        public async Task<bool> UpdateRefreshToken(UpdateRefreshToken model)
        {
            try
            {
                var user = GetUserById(model.UserId);
                user.RefreshToken = model.RefreshToken;
                if (model.RefreshTokenExpiryTime.HasValue)
                {
                    user.RefreshTokenExpiryTime = model.RefreshTokenExpiryTime.Value;
                }

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string WriteJwtToken(string userId, DateTime expiredTime)
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", userId)};

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: expiredTime,
                        signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    public interface IUserService
    {
        ApplicationUser GetUserByEmail(string email);
        ApplicationUser GetUserByUserName(string userName);
        ApplicationUser GetUserById(string userId);
        Task<ResultRegisterModel> AddUser(RegisterBody model);
        bool CheckExistUserByEmail(string email);
        bool CheckExistUserByPhoneNumber(string phoneNumber);
        bool CheckExistUserByUserName(string userName);
        bool CheckExistUserById(string userId);
        string WriteJwtToken(string userId, DateTime expiredTime);
        Task<ResultLoginModel> Login(LoginBody model);
        Task<bool> UpdateRefreshToken(UpdateRefreshToken model);
    }
}
