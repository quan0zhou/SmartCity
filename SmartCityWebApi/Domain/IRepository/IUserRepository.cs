using SmartCityWebApi.Infrastructure;

namespace SmartCityWebApi.Domain.IRepository
{
    public interface IUserRepository
    {
        /// <summary>
        /// 初始化admin用户
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> InitData();

        ValueTask<User?> Login(string account, string pwd);

        ValueTask<int[]> GetUserPermission(long userId);

        ValueTask<bool> ExistsUser(long userId, string pwd);

        ValueTask<bool> ChangeUserPwd(long userId, string pwd);

        ValueTask<(IEnumerable<dynamic>, int)> UserPageList(string keyword, int pageNo, int pageSize);

        ValueTask<bool> ExistsUserAccount(long? userId, string userAccount);

        ValueTask<(bool, string)> SaveUser(bool isEdit, User user,List<int> pers);

        ValueTask<dynamic?> Info(long userId);

        ValueTask<int[]> GetUserPers(long userId);
    }
}
