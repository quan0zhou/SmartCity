using IdGen;
using Medallion.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Models;

namespace SmartCityWebApi.Controllers.Mobile
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class CustSpaceController : ControllerBase
    {
        private readonly ICustSpaceRepository _custSpaceRepository;
        private readonly IHttpClientFactory _httpClientFactory;


        public CustSpaceController(ICustSpaceRepository custSpaceRepository, IHttpClientFactory httpClientFactory)
        {
            _custSpaceRepository = custSpaceRepository;
            _httpClientFactory = httpClientFactory;

        }
        [HttpGet("Info")]
        public async ValueTask<MobileResModel> Info()
        {
            MobileResModel mobileResModel = new MobileResModel();
            mobileResModel.Status = true;
            var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
            mobileResModel.Data = new { Title = spaceSetting?.ReservationTitle ?? string.Empty, AppID = spaceSetting?.AppID ?? string.Empty };
            return mobileResModel;
        }

        [HttpGet("OpenId/{code}")]
        public async ValueTask<MobileResModel> GetOpenId(string code)
        {
            MobileResModel mobileResModel = new MobileResModel();
            mobileResModel.Msg = "获取OpenId失败";
            string openId = string.Empty;
            var spaceSetting = await _custSpaceRepository.GetCustSpaceSettingInfo();
            if (spaceSetting != null)
            {
                string appId = spaceSetting.AppID;
                string appSecret = spaceSetting.AppSecret;
                using (var client = _httpClientFactory.CreateClient())
                {
                    var result = await client.GetFromJsonAsync<Dictionary<string, object>>($"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appId}&secret={appSecret}&code={code}&grant_type=authorization_code");
                    if (result != null)
                    {
                        if (result.ContainsKey("openid"))
                        {
                            openId = result["openid"].ToString()!;
                            mobileResModel.Status = true;
                        }
                        else if (result.ContainsKey("errmsg"))
                        {
                            mobileResModel.Msg = result["errmsg"].ToString()!;
                        }

                    }
                }

            }
            mobileResModel.Data = openId;
            return mobileResModel;
        }


    }
}
