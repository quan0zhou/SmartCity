
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Models;
using SmartCityWebApi.ViewModels;

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

        [HttpPost("ChangePwd")]
        public async ValueTask<IActionResult> ChangePwd(ChangePwdViewModel changePwdViewModel) 
        {
            changePwdViewModel = changePwdViewModel ?? new ChangePwdViewModel();
            changePwdViewModel.OldPassword = (changePwdViewModel.OldPassword ?? "").Trim();
            changePwdViewModel.Password = (changePwdViewModel.Password ?? "").Trim();
            changePwdViewModel.ConfirmPassword = (changePwdViewModel.ConfirmPassword ?? "").Trim();
            if (changePwdViewModel.OldPassword.Length<=0) 
            {
                return this.Ok(new {status=false,msg="原密码不能为空" });
            }
            if (changePwdViewModel.Password.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "新密码不能为空" });
            }
            if (changePwdViewModel.ConfirmPassword.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "再次输入密码不能为空" });
            }
            if (changePwdViewModel.Password!= changePwdViewModel.ConfirmPassword)
            {
                return this.Ok(new { status = false, msg = "两次输入密码不一致" });
            }
            var user = this.CurrentUser;
            if (!await _userRepository.ExistsUser(user.UserId, changePwdViewModel.OldPassword.ToSha256Encrypt()))
            {
                return this.Ok(new { status = false, msg = "输入的原密码不正确" });
            }
            if (await _userRepository.ChangeUserPwd(user.UserId, changePwdViewModel.Password.ToSha256Encrypt()))
            {
                return this.Ok(new { status = true, msg = "密码修改成功" });
            }
            else
            {
                return this.Ok(new { status = false, msg = "密码修改失败" });
            }
          
        }

    }
}
