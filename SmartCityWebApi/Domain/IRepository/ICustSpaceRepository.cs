using SmartCityWebApi.Models;

namespace SmartCityWebApi.Domain.IRepository
{
    public interface ICustSpaceRepository
    {
        ValueTask<CustSpaceSettingModel?> GetCustSpaceSettingInfo();
        ValueTask<(bool, string)> CustSpaceSettingSave(CustSpaceSetting custSpaceSetting);

        ValueTask<(IEnumerable<dynamic>, int)> CustSpacePageList(string spaceName, string contactName, int? spaceType, int pageNo, int pageSize);

        ValueTask<(bool, string)> CustSpaceSave(CustSpace custSpace);

        ValueTask<dynamic?> Info(long spaceId);

        ValueTask<(bool, string)> Delete(long spaceId);

        ValueTask<IEnumerable<dynamic>> CustSpaceList(int spaceType);
    }
}
