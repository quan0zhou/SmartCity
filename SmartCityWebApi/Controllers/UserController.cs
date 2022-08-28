
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Models;

namespace SmartCityWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : AuthorizeController
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository) 
        {
            _userRepository=userRepository;
        }

        [HttpGet("Info")]
        public IActionResult Info() 
        {
           
            return this.Ok(new { user = this.CurrentUser });
        }

        [HttpGet("Nav")]
        public async ValueTask<IActionResult> Nav() 
        {
            int[] pers = new int[] { };
            if (!this.CurrentUser.IsAdmin)
            {
                pers = await _userRepository.GetUserPermission(this.CurrentUser.UserId);
            }
            return this.Ok(new { result=Menu.MenuList });
        }

    }
}
