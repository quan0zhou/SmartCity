using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartCityWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly ILogger<AccountController> _logger;

        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, IUserRepository userRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async ValueTask<IActionResult> Login(LoginUserViewModel loginUserViewModel)
        {
            string pwd = loginUserViewModel.Password.ToSha256Encrypt();
            var user =await _userRepository.Login(loginUserViewModel.UserName, pwd);
            if (user!=null)
            {
                //create claims details based on the user information
                var claims = new[] {
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("UserAccount", user.UserAccount),
                        new Claim("UserName", user.UserName),
                        new Claim("IsAdmin", user.IsAdmin?"1":"0")
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtToken:SecretKey"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JwtToken:Issuer"],
                    _configuration["JwtToken:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: signIn);
                return Ok(new { token= new JwtSecurityTokenHandler().WriteToken(token) } );
            }
            else
            {
                return BadRequest(new { message= "账号或密码不正确" });
            }
        }

        [HttpPost("LogOut")]
        public IActionResult LogOut() 
        {
            return Ok();
        }
    }
}