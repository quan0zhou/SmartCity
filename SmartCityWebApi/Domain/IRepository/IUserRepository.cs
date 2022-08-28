namespace SmartCityWebApi.Domain.IRepository
{
    public interface IUserRepository
    {
        /// <summary>
        /// 初始化admin用户
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> InitData();

        ValueTask<User?> Login(string account,string pwd);

        ValueTask<int[]> GetUserPermission(long UserId);
    }
}
