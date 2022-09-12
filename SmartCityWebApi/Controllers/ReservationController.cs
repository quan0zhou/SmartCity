using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;

namespace SmartCityWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationController : AuthorizeController
    {

        private readonly IReservationRepository  _reservationRepository;
        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        [HttpGet("List")]
        public async ValueTask<IActionResult> List()
        {
            var list= await _reservationRepository.GetReservationList(DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
            return this.Ok(new { data = list.ToLookup(r => r.ReservationDate) });
        }

    }
}
