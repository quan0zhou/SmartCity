using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Infrastructure.Repository
{
    public class CustSpaceRepository : ICustSpaceRepository
    {
        private readonly SmartCityContext _smartCityContext;
        private readonly IdGenerator _idGenerator;
        public CustSpaceRepository(SmartCityContext smartCityContext, IdGenerator idGenerator)
        {
            _smartCityContext = smartCityContext;
            _idGenerator = idGenerator;
        }
        public async ValueTask<dynamic?> GetCustSpaceSettingInfo()
        {
            return await _smartCityContext.CustSpaceSettings.AsNoTracking().Select(r => new
            {
                CustId = r.CustId.ToString(),
                r.ReservationTitle,
                r.StartTime,
                r.EndTime,
                r.TimePeriod,
                r.SettableDays,
                r.BookableDays,
                r.DirectRefundPeriod,
                r.AppID,
                r.MchID,
                r.SubMchID,
                r.AppKey,
                r.AppSecret

            }).FirstOrDefaultAsync();
        }

        public async ValueTask<(bool, string)> CustSpaceSettingSave(CustSpaceSetting custSpaceSetting)
        {

            if (custSpaceSetting.CustId > 0)
            {
                var model = await _smartCityContext.CustSpaceSettings.Where(r => r.CustId.Equals(custSpaceSetting.CustId)).FirstOrDefaultAsync();
                if (model == null)
                {
                    return (false, "该场地设置不存在");
                }
                model.ReservationTitle = custSpaceSetting.ReservationTitle;
                model.StartTime = custSpaceSetting.StartTime;
                model.EndTime = custSpaceSetting.EndTime;
                model.EndTime = custSpaceSetting.EndTime;
                model.TimePeriod = custSpaceSetting.TimePeriod;
                model.SettableDays = custSpaceSetting.SettableDays;
                model.BookableDays = custSpaceSetting.BookableDays;
                model.DirectRefundPeriod = custSpaceSetting.DirectRefundPeriod;
                model.AppID = custSpaceSetting.AppID;
                model.MchID = custSpaceSetting.MchID;
                model.SubMchID = custSpaceSetting.SubMchID;
                model.AppKey = custSpaceSetting.AppKey;
                model.AppSecret = custSpaceSetting.AppSecret;
                model.UpdateUser = custSpaceSetting.UpdateUser;
                model.UpdateTime = custSpaceSetting.UpdateTime;
            }
            else
            {
                custSpaceSetting.CustId = _idGenerator.CreateId();
                _smartCityContext.CustSpaceSettings.Add(custSpaceSetting);
            }
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (result, result ? "保存成功" : "保存失败");

        }


        public async ValueTask<(IEnumerable<dynamic>, int)> CustSpacePageList(string spaceName,string contactName,int? spaceType, int pageNo, int pageSize)
        {
            var query = _smartCityContext.CustSpaces.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(spaceName))
            {
                query = query.Where(r => r.SpaceName.Contains(spaceName));
            }
            if (!string.IsNullOrWhiteSpace(contactName))
            {
                query = query.Where(r => r.ContactName.Contains(contactName));
            }
            if (spaceType>0)
            {
                query = query.Where(r => r.SpaceType.Equals(spaceType.Value));
            }
            var count = await query.CountAsync();
            var list = await query.OrderByDescending(r => r.UpdateTime).Skip(pageSize * (pageNo - 1)).Take(pageSize).Select(r => new
            {
                SpaceId = r.SpaceId.ToString(),
                r.SpaceName,
                r.SpaceAddress,
                r.ContactPhone,
                r.ContactName,
                r.Remark,
                r.SpaceType,
                UpdateTime = r.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

            }).ToListAsync();
            return (list, count);
        }

        public async ValueTask<(bool, string)> CustSpaceSave(CustSpace custSpace) 
        {
            if (custSpace.SpaceId > 0)
            {
                var model = await _smartCityContext.CustSpaces.Where(r => r.SpaceId.Equals(custSpace.SpaceId)).FirstOrDefaultAsync();
                if (model == null)
                {
                    return (false, "该场地不存在");
                }
                if (await _smartCityContext.CustSpaces.AnyAsync(r=>r.SpaceName==custSpace.SpaceName&&r.SpaceType==custSpace.SpaceType&&r.SpaceId!=custSpace.SpaceId))
                {
                    return (false, "该场地名称已存在");
                }
                model.ContactName = custSpace.ContactName;
                model.SpaceName = custSpace.SpaceName;
                model.SpaceAddress = custSpace.SpaceAddress;
                model.Remark = custSpace.Remark;
                model.SpaceType = custSpace.SpaceType;
                model.ContactPhone = custSpace.ContactPhone;
                model.CreateUser = custSpace.CreateUser;
                model.UpdateTime = custSpace.UpdateTime;
        
            }
            else
            {
                if (await _smartCityContext.CustSpaces.AnyAsync(r => r.SpaceName == custSpace.SpaceName && r.SpaceType == custSpace.SpaceType))
                {
                    return (false, "该场地名称已存在");
                }
                custSpace.SpaceId = _idGenerator.CreateId();
                _smartCityContext.CustSpaces.Add(custSpace);
            }
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (result, result ? "保存成功" : "保存失败");
        }
    }
}
