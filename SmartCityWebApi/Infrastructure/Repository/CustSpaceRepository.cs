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

        public async ValueTask<(bool, string)> SaveCustSpaceSetting(CustSpaceSetting custSpaceSetting)
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
    }
}
