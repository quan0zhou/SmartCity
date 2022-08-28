using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartCityWebApi.Controllers
{

    public struct ApplicationUser
    {
        public string UserName { get; set; }

        public string UserAccount { get; set; }

        public long UserId { get; set; }

        public bool IsAdmin { get; set; }

    }

    [Authorize]
    public class AuthorizeController : ControllerBase
    {

        private ApplicationUser InitUser() 
        {
            var model = new ApplicationUser();
            model.UserId = long.Parse(User.FindFirstValue("UserId"));
            model.UserName = User.FindFirstValue("UserName");
            model.UserAccount = User.FindFirstValue("UserAccount");
            model.IsAdmin = User.FindFirstValue("IsAdmin") == "1";
            return model;

        }
        protected ApplicationUser CurrentUser => InitUser();
    }
}
