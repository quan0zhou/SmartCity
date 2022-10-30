
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Infrastructure.Repository;
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
            _userRepository = userRepository;
        }

        [HttpGet("Info")]
        public IActionResult Info()
        {

            return this.Ok(new { user = this.CurrentUser });
        }

        [HttpGet("Nav")]
        public async ValueTask<IActionResult> Nav()
        {
            List<Menu> menuList = Menu.MenuList;
            if (!this.CurrentUser.IsAdmin)
            {
                var pers = await _userRepository.GetUserPermission(this.CurrentUser.UserId);
                if (pers != null && pers.Count > 0)
                {
                    if (!pers.Any(r => r.Equals(1000))) 
                    {
                        pers.Add(1000);
                    }
                    var menus = menuList.Where(r => pers.Contains(r.Id)).GroupBy(r => r.ParentId).Select(r=>r.Key);
                    menuList = menuList.Where(r => pers.Any(p => p.Equals(r.Id))|| menus.Any(p=>p.Equals(r.Id))).ToList();
                }

            }
            return this.Ok(new { result = menuList });
        }

        [HttpPatch("Pwd")]
        public async ValueTask<IActionResult> ChangePwd(ChangePwdViewModel changePwdViewModel)
        {
            changePwdViewModel = changePwdViewModel ?? new ChangePwdViewModel();
            changePwdViewModel.OldPassword = (changePwdViewModel.OldPassword ?? "").Trim();
            changePwdViewModel.Password = (changePwdViewModel.Password ?? "").Trim();
            changePwdViewModel.ConfirmPassword = (changePwdViewModel.ConfirmPassword ?? "").Trim();
            if (changePwdViewModel.OldPassword.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "原密码不能为空" });
            }
            if (changePwdViewModel.Password.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "新密码不能为空" });
            }
            if (changePwdViewModel.ConfirmPassword.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "再次输入密码不能为空" });
            }
            if (changePwdViewModel.Password != changePwdViewModel.ConfirmPassword)
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

        [HttpPost("List")]
        public async ValueTask<IActionResult> List(UserPageViewModel userPageView)
        {
            userPageView = userPageView ?? new UserPageViewModel();
            userPageView.Keyword = (userPageView.Keyword ?? "").Trim();
            userPageView.PageSize = userPageView.PageSize <= 10 ? 10 : userPageView.PageSize;
            userPageView.PageNo = userPageView.PageNo <= 1 ? 1 : userPageView.PageNo;
            var (list, count) = await _userRepository.UserPageList(userPageView.Keyword, userPageView.PageNo, userPageView.PageSize);
            return this.Ok(new { data = list, pageSize = userPageView.PageSize, pageNo = userPageView.PageNo, totalPage = count / userPageView.PageSize, totalCount = count });
        }

        [HttpPost("Save")]
        public async ValueTask<IActionResult> Save(UserViewModel user)
        {
            user = user ?? new UserViewModel();
            user.UserName = (user.UserName ?? "").Trim();
            user.UserAccount = (user.UserAccount ?? "").Trim();
            user.UserAccountPwd = (user.UserAccountPwd ?? "").Trim();
            user.Remark = (user.Remark ?? "").Trim();
            user.ContactPhone = (user.ContactPhone ?? "").Trim();
            user.Pers = user.Pers ?? new List<int>();
            if (user.UserName.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "用户名不能为空" });
            }
            if (user.UserAccount.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "用户账号不能为空" });
            }
            bool isEdit = true;
            if (!user.UserId.HasValue)
            {
                if (user.UserAccountPwd.Length <= 0)
                {
                    return this.Ok(new { status = false, msg = "账号密码不能为空" });
                }
                isEdit = false;
                user.UserAccountPwd = user.UserAccountPwd.ToSha256Encrypt();
            }
            if (await _userRepository.ExistsUserAccount(user.UserId, user.UserAccount))
            {
                return this.Ok(new { status = false, msg = "该用户账号已存在" });
            }
            var currentUser = this.CurrentUser;
            var (result, msg) = await _userRepository.SaveUser(isEdit, new Domain.User
            {
                UserId = isEdit ? user.UserId!.Value : 0,
                IsAdmin = false,
                UserName = user.UserName,
                ContactPhone = user.ContactPhone,
                CreateTime = DateTime.Now,
                CreateUser = currentUser.UserId,
                Remark = user.Remark,
                UpdateTime = DateTime.Now,
                UpdateUser = currentUser.UserId,
                UserAccount = user.UserAccount,
                UserAccountPwd = user.UserAccountPwd
            }, user.Pers);
            return this.Ok(new { status = result, msg });
        }

        [HttpGet("{userId:long}")]
        public async ValueTask<IActionResult> Info(long userId)
        {
            var model = await _userRepository.Info(userId);
            if (model == null)
            {
                return this.Ok(new { status = false, msg = "该用户不存在" });
            }
            var pers = await _userRepository.GetUserPers(userId);
            return this.Ok(new { status = true, data = model, pers });
        }

        [HttpDelete("{userId:long}")]
        public async ValueTask<IActionResult> Delete(long userId)
        {
            var (result, msg) = await _userRepository.Delete(userId);
            return this.Ok(new { status = result, msg });
        }

        [HttpPatch("Reset/{userId:long}")]
        public async ValueTask<IActionResult> ResetPwd(long userId)
        {
            var (result, msg) = await _userRepository.ResetPwd(userId, "qwer123".ToMd5().ToSha256Encrypt(), this.CurrentUser.UserId);
            return this.Ok(new { status = result, msg });
        }


    }
}
