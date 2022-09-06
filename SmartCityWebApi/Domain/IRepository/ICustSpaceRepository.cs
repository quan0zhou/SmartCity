namespace SmartCityWebApi.Domain.IRepository
{
    public interface ICustSpaceRepository
    {
        ValueTask<dynamic?> GetCustSpaceSettingInfo();
        ValueTask<(bool, string)> SaveCustSpaceSetting(CustSpaceSetting custSpaceSetting);
    }
}
