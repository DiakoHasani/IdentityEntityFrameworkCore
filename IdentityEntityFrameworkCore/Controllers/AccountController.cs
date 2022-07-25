using IdentityEntityFrameworkCore.Models.Services;
using IdentityEntityFrameworkCore.Models.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace IdentityEntityFrameworkCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginBody model)
        {
            var resultLogin = await _userService.Login(model);
            if (!resultLogin.Result)
            {
                return BadRequest(resultLogin.Message);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, resultLogin.UserId) };
            foreach (var role in resultLogin.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            if (!await _userService.UpdateRefreshToken(new UpdateRefreshToken
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
                UserId = resultLogin.UserId
            }))
            {
                return BadRequest("خطای رخ داده است");
            }

            return Ok(new AuthenticatedResponseModel
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] RegisterBody model)
        {
            var resultAddUser = await _userService.AddUser(model);

            if (!resultAddUser.Result)
            {
                return BadRequest(resultAddUser.Message);
            }

            return Ok("عملیات با موفقیت انجام شد");
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenApiModel.AccessToken);
            var userId = principal.Identity.Name;

            if (!_userService.CheckExpireLoginAndRefreshToken(userId, tokenApiModel.RefreshToken))
            {
                return Unauthorized("لطفا لاگین کنید");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            if (!await _userService.UpdateRefreshToken(new UpdateRefreshToken
            {
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = null,
                UserId = userId
            }))
            {
                return BadRequest("خطای رخ داده است");
            }

            return Ok(new AuthenticatedResponseModel()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("Revoke")]
        public async Task<IActionResult> Revoke()
        {
            var userId = User.Identity.Name;
            if (!_userService.CheckExistUserById(userId))
            {
                return NotFound("چنین کاربری یافت نشد");
            }

            if (!await _userService.UpdateRefreshToken(new UpdateRefreshToken
            {
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                UserId = userId
            }))
            {
                return BadRequest("خطای رخ داده است");
            }

            return NoContent();
        }
    }
}
