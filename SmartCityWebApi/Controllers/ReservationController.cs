using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Models;
using SmartCityWebApi.ViewModels;

namespace SmartCityWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationController : AuthorizeController
    {

        private readonly IReservationRepository _reservationRepository;
        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        [HttpGet("TagList")]
        public async ValueTask<IActionResult> TagList()
        {
            var list = await _reservationRepository.GetReservationList(DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd")),false);
            var query = list.GroupBy(r => r.ReservationDate);
            var data = new List<ReservationTag>();
            foreach (var item in query)
            {
                var tag = new ReservationTag
                {
                    Date = item.Key.ToString("yyyy-MM-dd"),
                    Week = item.Key.ToWeekName()
                };
                var items = item.OrderBy(r => r.StartTime);
                foreach (var reservation in items)
                {
                    tag.Items.Add(new ReservationItem
                    {
                        ReservationId = reservation.ReservationId.ToString(),
                        StartTime = reservation.StartTime,
                        EndTime = reservation.EndTime,
                        Money = reservation.Money,
                        ReservationDate = reservation.ReservationDate.ToString("yyyy-MM-dd"),
                        ReservationStatus = reservation.ReservationStatus,
                        IsBooked = reservation.IsBooked,
                        SpaceId = reservation.SpaceId.ToString(),
                        SpaceType = reservation.SpaceType,
                        SpaceName = reservation.SpaceName

                    });
                }
                tag.InitStatus(false);
                data.Add(tag);
            }

            return this.Ok(data);
        }

        [HttpGet("Tag/{date:datetime}")]
        public async ValueTask<IActionResult> Tag(DateTime date)
        {
            var list = await _reservationRepository.GetReservationList(DateOnly.Parse(date.ToString("yyyy-MM-dd")),true);
            var query = list.GroupBy(r => r.ReservationDate);
            ReservationTag? tag = null;
            foreach (var item in query)
            {
                tag = new ReservationTag
                {
                    Date = item.Key.ToString("yyyy-MM-dd"),
                    Week = item.Key.ToWeekName()
                };
                var items = item.OrderBy(r => r.StartTime);
                foreach (var reservation in items)
                {
                    tag.Items.Add(new ReservationItem
                    {
                        ReservationId = reservation.ReservationId.ToString(),
                        StartTime = reservation.StartTime,
                        EndTime = reservation.EndTime,
                        Money = reservation.Money,
                        ReservationDate = reservation.ReservationDate.ToString("yyyy-MM-dd"),
                        ReservationStatus = reservation.ReservationStatus,
                        IsBooked = reservation.IsBooked,
                        SpaceId = reservation.SpaceId.ToString(),
                        SpaceType = reservation.SpaceType,
                        SpaceName = reservation.SpaceName

                    });
                }
                tag.InitStatus(false);
            }

            return this.Ok(tag);
        }

        [HttpPatch("SetUnreservable")]
        public async ValueTask<IActionResult> SetUnreservable(ReservationItemViewModel[] itemViewModels)
        {
            if (itemViewModels == null || itemViewModels.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "选择的预订记录为空" });
            }
            var result = await _reservationRepository.SetReservationStatus(itemViewModels.Select(r => r.ReservationId).ToArray(), true);
            if (result)
            {
                return this.Ok(new { status = true, msg = "设置成功" });
            }
            else
            {
                return this.Ok(new { status = false, msg = "设置失败" });
            }
        }

        [HttpPatch("SetReservable")]
        public async ValueTask<IActionResult> SetReservable(ReservationItemViewModel[] itemViewModels)
        {
            if (itemViewModels == null || itemViewModels.Length <= 0)
            {
                return this.Ok(new { status = false, msg = "选择的预订记录为空" });
            }
            var result = await _reservationRepository.SetReservationStatus(itemViewModels.Select(r => r.ReservationId).ToArray(), false);
            if (result)
            {
                return this.Ok(new { status = true, msg = "设置成功" });
            }
            else
            {
                return this.Ok(new { status = false, msg = "设置失败" });
            }
        }


        [HttpPatch("SetMoney")]
        public async ValueTask<IActionResult> SetMoney(ReservationListItemViewModel itemViewModels)
        {
            if (itemViewModels == null || itemViewModels.Items ==null|| itemViewModels.Items.Length<=0)
            {
                return this.Ok(new { status = false, msg = "选择的预订记录为空" });
            }
            if (itemViewModels.Money <= 0) 
            {
                return this.Ok(new { status = false, msg = "预订金额必须大于0" });
            }
            var result = await _reservationRepository.SetReservationMoney(itemViewModels.Items.Select(r => r.ReservationId).ToArray(), itemViewModels.Money);
            if (result)
            {
                return this.Ok(new { status = true, msg = "设置成功" });
            }
            else
            {
                return this.Ok(new { status = false, msg = "设置失败" });
            }
        }

    }
}
