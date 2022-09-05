using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using System;

namespace SmartCityWebApi.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SmartCityContext _smartCityContext;
        private readonly IdGenerator _idGenerator;
        public UserRepository(SmartCityContext smartCityContext, IdGenerator idGenerator)
        {
            _smartCityContext = smartCityContext;
            _idGenerator = idGenerator;
        }

        public async ValueTask<List<int>> GetUserPermission(long userId)
        {
            return await _smartCityContext.UserPermissions.AsNoTracking().Where(r => r.UserId.Equals(userId)).Select(r => r.PageId).ToListAsync();
        }

        public async ValueTask<bool> InitData()
        {

            if (!await _smartCityContext.Users.AnyAsync(r => r.UserAccount == "admin"))
            {
                User usrData = new User
                {
                    UserId = _idGenerator.CreateId(),
                    UserAccount = "admin",
                    UserAccountPwd = "admin000".ToMd5().ToSha256Encrypt(),
                    ContactPhone = string.Empty,
                    CreateTime = DateTime.Now,
                    CreateUser = 0,
                    IsAdmin = true,
                    Remark = string.Empty,
                    UpdateTime = DateTime.Now,
                    UpdateUser = 0,
                    UserName = "管理员"

                };
                await _smartCityContext.Users.AddAsync(usrData);
                return await _smartCityContext.SaveChangesAsync() > 0;
            }

            return false;

        }

        public async ValueTask<User?> Login(string account, string pwd)
        {
            return await _smartCityContext.Users.AsNoTracking().Where(r => r.UserAccount.Equals(account) && r.UserAccountPwd.Equals(pwd)).FirstOrDefaultAsync();
        }

        public async ValueTask<bool> ExistsUser(long userId, string pwd)
        {
            return await _smartCityContext.Users.AnyAsync(r => r.UserId.Equals(userId) && r.UserAccountPwd.Equals(pwd));
        }

        public async ValueTask<bool> ChangeUserPwd(long userId, string pwd)
        {
            return await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE \"user\"  SET \"UserAccountPwd\" ={pwd},\"UpdateUser\"={userId},\"UpdateTime\"=now() WHERE \"UserId\"={userId}") > 0;
        }
        public async ValueTask<(IEnumerable<dynamic>, int)> UserPageList(string keyword, int pageNo, int pageSize)
        {
            var query = _smartCityContext.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r => r.UserName.Contains(keyword) || r.UserAccount.Contains(keyword));
            }
            var count = await query.CountAsync();
            var list = await query.OrderByDescending(r => r.UpdateTime).Skip(pageSize * (pageNo - 1)).Take(pageSize).Select(r => new
            {
                UserId = r.UserId.ToString(),
                r.UserName,
                r.UserAccount,
                r.ContactPhone,
                r.IsAdmin,
                r.Remark,
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToListAsync();
            return (list, count);
        }

        public async ValueTask<bool> ExistsUserAccount(long? userId, string userAccount)
        {
            var query = _smartCityContext.Users.Where(r => r.UserAccount.Equals(userAccount));
            if (userId.HasValue)
            {
                return await query.Where(r => r.UserId!=userId.Value).AnyAsync();
            }
            else
            {
                return await query.AnyAsync();
            }
        }

        public async ValueTask<(bool, string)> SaveUser(bool isEdit, User user, List<int> pers)
        {
            if (isEdit)
            {
                var model = await _smartCityContext.Users.FirstOrDefaultAsync(r => r.UserId.Equals(user.UserId));
                if (model == null)
                {
                    return (false, "该用户不存在");
                }
                model.UpdateUser = user.UpdateUser;
                model.UpdateTime = user.UpdateTime;
                model.UserAccount = user.UserAccount;
                model.UserName = user.UserName;
                model.ContactPhone = user.ContactPhone;
                model.Remark = user.Remark;
                await _smartCityContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"userPermission\" WHERE \"UserId\"={user.UserId}");
                foreach (var item in pers)
                {
                    _smartCityContext.UserPermissions.Add(new UserPermission
                    {
                        PageId = item,
                        UserId = user.UserId,
                        UserPermissionId = _idGenerator.CreateId()

                    });
                }
                var result = await _smartCityContext.SaveChangesAsync() > 0;
                return (result, result ? "保存成功" : "保存失败");
            }
            else
            {
                user.UserId = _idGenerator.CreateId();
                _smartCityContext.Users.Add(user);
                await _smartCityContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"userPermission\" WHERE \"UserId\"={user.UserId}");
                foreach (var item in pers)
                {
                    _smartCityContext.UserPermissions.Add(new UserPermission
                    {
                        PageId = item,
                        UserId = user.UserId,
                        UserPermissionId = _idGenerator.CreateId()

                    });
                }
                var result = await _smartCityContext.SaveChangesAsync() > 0;
                return (result, result ? "保存成功" : "保存失败");
            }

        }

        public async ValueTask<dynamic?> Info(long userId)
        {

            return await _smartCityContext.Users.AsNoTracking().Where(r => r.UserId.Equals(userId)).Select(r => new
            {
                UserId = r.UserId.ToString(),
                r.UserAccount,
                r.IsAdmin,
                r.ContactPhone,
                r.UserName,
                r.Remark,
            }).FirstOrDefaultAsync();

        }

        public async ValueTask<int[]> GetUserPers(long userId)
        {
            return await _smartCityContext.UserPermissions.Where(r => r.UserId.Equals(userId)).Select(r => r.PageId).ToArrayAsync();
        }

        public async ValueTask<(bool, string)> Delete(long userId) 
        {
            var model = await _smartCityContext.Users.FirstOrDefaultAsync(r => r.UserId.Equals(userId));
            if (model == null)
            {
                return (false, "该用户不存在");
            }
            _smartCityContext.Users.Remove(model);
            await _smartCityContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"userPermission\" WHERE \"UserId\"={userId}");
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (result, result ? "删除成功" : "删除失败");

        }

        public async ValueTask<(bool, string)> ResetPwd(long userId, string pwd,long updateUser) 
        {
            var model = await _smartCityContext.Users.FirstOrDefaultAsync(r => r.UserId.Equals(userId));
            if (model == null)
            {
                return (false, "该用户不存在");
            }
            model.UpdateUser = updateUser;
            model.UpdateTime = DateTime.Now;
            model.UserAccountPwd = pwd;
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (result, result ? "重置成功" : "重置失败");
        }
    }
}
