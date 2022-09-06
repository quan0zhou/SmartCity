using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
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
            var (result, msg) = await _custSpaceRepository.SaveCustSpaceSetting(new Domain.CustSpaceSetting
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
    }
}
