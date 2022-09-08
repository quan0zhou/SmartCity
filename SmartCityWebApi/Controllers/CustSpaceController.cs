using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Infrastructure.Repository;
using SmartCityWebApi.ViewModels;

namespace SmartCityWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustSpaceController : AuthorizeController
    {
        public readonly ICustSpaceRepository _custSpaceRepository;

        public CustSpaceController(ICustSpaceRepository custSpaceRepository)
        {
            _custSpaceRepository = custSpaceRepository;
        }
        [HttpGet("Setting/Info")]
        public async ValueTask<IActionResult> Setting()
        {
            return this.Ok(await _custSpaceRepository.GetCustSpaceSettingInfo());
        }
        [HttpPost("Setting/Save")]
        public async ValueTask<IActionResult> SettingSave(CustSpaceSettingViewModel custSpaceSetting)
        {
            custSpaceSetting = custSpaceSetting ?? new CustSpaceSettingViewModel();
            custSpaceSetting.AppID = (custSpaceSetting.AppID ?? "").Trim();
            custSpaceSetting.AppSecret = (custSpaceSetting.AppSecret ?? "").Trim();
            custSpaceSetting.MchID = (custSpaceSetting.MchID ?? "").Trim();
            custSpaceSetting.SubMchID = (custSpaceSetting.SubMchID ?? "").Trim();

            var user = this.CurrentUser;
            var (result, msg) = await _custSpaceRepository.CustSpaceSettingSave(new Domain.CustSpaceSetting
            {
                CustId = custSpaceSetting.CustId.HasValue ? custSpaceSetting.CustId.Value : 0,
                AppID = custSpaceSetting.AppID,
                AppKey = custSpaceSetting.AppKey,
                AppSecret = custSpaceSetting.AppSecret,
                ReservationTitle = custSpaceSetting.ReservationTitle,
                SettableDays = custSpaceSetting.SettableDays,
                BookableDays = custSpaceSetting.BookableDays,
                DirectRefundPeriod = custSpaceSetting.DirectRefundPeriod,
                StartTime = custSpaceSetting.StartTime,
                EndTime = custSpaceSetting.EndTime,
                MchID = custSpaceSetting.MchID,
                SubMchID = custSpaceSetting.SubMchID,
                TimePeriod = custSpaceSetting.TimePeriod,
                CreateTime = DateTime.Now,
                CreateUser = user.UserId,
                UpdateTime = DateTime.Now,
                UpdateUser = user.UserId

            });

            return this.Ok(new { status = result, msg });


        }

        [HttpPost("List")]
        public async Task<IActionResult> CustSpaceList(CustSpacePageViewModel custSpacePageViewModel) 
        {
            custSpacePageViewModel = custSpacePageViewModel ?? new CustSpacePageViewModel();
            custSpacePageViewModel.SpaceName = (custSpacePageViewModel.SpaceName ?? "").Trim();
            custSpacePageViewModel.ContactName = (custSpacePageViewModel.ContactName ?? "").Trim();
            custSpacePageViewModel.PageSize = custSpacePageViewModel.PageSize <= 10 ? 10 : custSpacePageViewModel.PageSize;
            custSpacePageViewModel.PageNo = custSpacePageViewModel.PageNo <= 1 ? 1 : custSpacePageViewModel.PageNo;
            var (list, count) = await _custSpaceRepository.CustSpacePageList(custSpacePageViewModel.SpaceName, custSpacePageViewModel.ContactName, custSpacePageViewModel.SpaceType, custSpacePageViewModel.PageNo, custSpacePageViewModel.PageSize);
            return this.Ok(new { data = list, pageSize = custSpacePageViewModel.PageSize, pageNo = custSpacePageViewModel.PageNo, totalPage = count / custSpacePageViewModel.PageSize, totalCount = count });
        }

        [HttpPost("Save")]
        public async Task<IActionResult> CustSpaceSave(CustSpaceViewModel custSpaceViewModel) 
        {
            custSpaceViewModel = custSpaceViewModel ?? new CustSpaceViewModel();
            custSpaceViewModel.SpaceName = (custSpaceViewModel.SpaceName ?? "").Trim();
            custSpaceViewModel.ContactName = (custSpaceViewModel.ContactName ?? "").Trim();
            custSpaceViewModel.SpaceAddress = (custSpaceViewModel.SpaceAddress ?? "").Trim();
            custSpaceViewModel.ContactPhone = (custSpaceViewModel.ContactPhone ?? "").Trim();
            custSpaceViewModel.Remark = (custSpaceViewModel.Remark ?? "").Trim();
            if (custSpaceViewModel.SpaceName.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "场地名称不能为空" });
            }
            if (custSpaceViewModel.SpaceType<=0)
            {
                return this.Ok(new { status = false, msg = "场地类型不能为空" });
            }
            var user = this.CurrentUser;
            var (result, msg) = await _custSpaceRepository.CustSpaceSave(new Domain.CustSpace
            {
                SpaceId = custSpaceViewModel.SpaceId.HasValue ? custSpaceViewModel.SpaceId.Value : 0,
                ContactName = custSpaceViewModel.ContactName,
                ContactPhone = custSpaceViewModel.ContactPhone,
                SpaceAddress = custSpaceViewModel.SpaceAddress,
                SpaceType = custSpaceViewModel.SpaceType,
                Remark= custSpaceViewModel.Remark,
                SpaceName = custSpaceViewModel.SpaceName,
                CreateTime = DateTime.Now,
                CreateUser = user.UserId,
                UpdateTime = DateTime.Now,
                UpdateUser = user.UserId

            });

            return this.Ok(new { status = result, msg });
        }
    }
}
