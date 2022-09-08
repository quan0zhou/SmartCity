namespace SmartCityWebApi.Domain.IRepository
{
    public interface ICustSpaceRepository
    {
        ValueTask<dynamic?> GetCustSpaceSettingInfo();
        ValueTask<(bool, string)> CustSpaceSettingSave(CustSpaceSetting custSpaceSetting);

        ValueTask<(IEnumerable<dynamic>, int)> CustSpacePageList(string spaceName, string contactName, int? spaceType, int pageNo, int pageSize);

        ValueTask<(bool, string)> CustSpaceSave(CustSpace custSpace);
    }
}
