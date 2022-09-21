using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Models;

namespace SmartCityWebApi.Controllers.Mobile
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class CustSpaceController : ControllerBase
    {
        public readonly ICustSpaceRepository _custSpaceRepository;

        public CustSpaceController(ICustSpaceRepository custSpaceRepository)
        {
            _custSpaceRepository = custSpaceRepository;
        }
        [HttpGet("Info")]
        public async ValueTask<MobileResModel> Info() 
        {
            MobileResModel mobileResModel = new MobileResModel();
            mobileResModel.Status = true;
            mobileResModel.Data = await _custSpaceRepository.GetCustSpaceSettingInfo();
            return mobileResModel;
        }

    }
}
