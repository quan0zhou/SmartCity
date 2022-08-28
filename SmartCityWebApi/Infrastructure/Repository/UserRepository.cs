using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;

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

        public async ValueTask<int[]> GetUserPermission(long UserId)
        {
            return await _smartCityContext.UserPermissions.AsNoTracking().Where(r => r.UserId.Equals(UserId)).Select(r => r.PageId).ToArrayAsync();
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
    }
}
