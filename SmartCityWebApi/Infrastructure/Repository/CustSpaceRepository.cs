using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateApplyForSubMerchantApplymentRequest.Types.Business.Types.SaleScene.Types;

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

        public async ValueTask<CustSpaceSettingModel?> GetCustSpaceSettingInfo()
        {
            return await _smartCityContext.CustSpaceSettings.AsNoTracking().Select(r => new CustSpaceSettingModel
            {
                CustId = r.CustId.ToString(),
                ReservationTitle=r.ReservationTitle,
                StartTime=r.StartTime,
                EndTime=r.EndTime,
                TimePeriod=r.TimePeriod,
                SettableDays=r.SettableDays,
                BookableDays = r.BookableDays,
                DirectRefundPeriod= r.DirectRefundPeriod,
                AppID= r.AppID,
                MchID= r.MchID,
                SubMchID=r.SubMchID,
                AppKey = r.AppKey,
                AppSecret= r.AppSecret,
                CertificatePrivateKey= r.CertificatePrivateKey,
                CertificateSerialNumber = r.CertificateSerialNumber

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
                model.CertificateSerialNumber = custSpaceSetting.CertificateSerialNumber;
                model.CertificatePrivateKey = custSpaceSetting.CertificatePrivateKey;
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

        public async ValueTask<dynamic?> Info(long spaceId)
        {
            return await _smartCityContext.CustSpaces.AsNoTracking().Where(r=>r.SpaceId.Equals(spaceId)).Select(r => new
            {
                SpaceId = r.SpaceId.ToString(),
                r.SpaceName,
                r.SpaceAddress,
                r.ContactPhone,
                r.ContactName,
                r.Remark,
                r.SpaceType,

            }).FirstOrDefaultAsync();
        }

        public async ValueTask<(bool, string)> Delete(long spaceId)
        {
            var model = await _smartCityContext.CustSpaces.FirstOrDefaultAsync(r => r.SpaceId.Equals(spaceId));
            if (model == null)
            {
                return (false, "该场地不存在");
            }
            _smartCityContext.CustSpaces.Remove(model);
            var result = await _smartCityContext.SaveChangesAsync() > 0;
            return (result, result ? "删除成功" : "删除失败");

        }

        public async ValueTask<IEnumerable<dynamic>> CustSpaceList(int spaceType)
        {
            return await _smartCityContext.CustSpaces.AsNoTracking().Where(r => r.SpaceType.Equals(spaceType)).OrderBy(r => r.SpaceName).Select(r => new
            {
                SpaceId=r.SpaceId.ToString(),
                r.SpaceName,
                r.SpaceType,
                r.SpaceAddress

            }).ToListAsync();
        }
    }
}
