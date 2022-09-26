using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Models;

namespace SmartCityWebApi.Controllers.Mobile
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        [HttpGet("TagList")]
        public async ValueTask<MobileResModel> TagList()
        {
            MobileResModel mobileResModel = new MobileResModel();
            var now = DateTime.Now;
            DateTime endDate;
            if (now.ToWeekName() == "星期一" && now >= Convert.ToDateTime("09:00"))
            {
                endDate = now.AddDays(7);
            }
            else
            {
                int week = (int)now.DayOfWeek;
                endDate = now.AddDays(7 - (week == 0 ? 7 : week) + 1);
            }
            var list = await _reservationRepository.GetReservationList(DateOnly.Parse(now.ToString("yyyy-MM-dd")), false, DateOnly.Parse(endDate.ToString("yyyy-MM-dd")));
            var query = list.GroupBy(r => r.ReservationDate).OrderBy(r => r.Key);
            var data = new List<ReservationTag>();
            foreach (var item in query)
            {
                var tag = new ReservationTag
                {
                    Date = item.Key.ToString("yyyy-MM-dd"),
                    Week = item.Key.ToWeekName()
                };
                var items = item.OrderBy(r => r.StartTime).ThenBy(r => r.SpaceName);
                foreach (var reservation in items)
                {
                    tag.Items.Add(new ReservationItem
                    {
                        ReservationId = reservation.ReservationId.ToString(),
                        StartTime = reservation.StartTime,
                        EndTime = reservation.EndTime,
                        Money = reservation.Money,
                        ReservationDate = reservation.ReservationDate.ToString("yyyy-MM-dd"),
                        ReservationStatus = reservation.IsBooked ? 0 : (now >= reservation.StartTime ? 0 : reservation.ReservationStatus),
                        IsBooked = reservation.IsBooked,
                        SpaceId = reservation.SpaceId.ToString(),
                        SpaceType = reservation.SpaceType,
                        SpaceName = reservation.SpaceName

                    });
                }
                tag.InitStatus(true);
                data.Add(tag);
            }
            mobileResModel.Status = true;
            mobileResModel.Data = data;
            return mobileResModel;
        }

        [HttpGet("Tag/{date:datetime}")]
        public async ValueTask<MobileResModel> Tag(DateTime date)
        {
            MobileResModel mobileResModel = new MobileResModel();
            var now = DateTime.Now;
            var list = await _reservationRepository.GetReservationList(DateOnly.Parse(date.ToString("yyyy-MM-dd")), true);
            var query = list.GroupBy(r => r.ReservationDate).OrderBy(r => r.Key);
            ReservationTag? tag = null;
            foreach (var item in query)
            {
                tag = new ReservationTag
                {
                    Date = item.Key.ToString("yyyy-MM-dd"),
                    Week = item.Key.ToWeekName()
                };
                var items = item.OrderBy(r => r.StartTime).ThenBy(r => r.SpaceName);
                foreach (var reservation in items)
                {
                    tag.Items.Add(new ReservationItem
                    {
                        ReservationId = reservation.ReservationId.ToString(),
                        StartTime = reservation.StartTime,
                        EndTime = reservation.EndTime,
                        Money = reservation.Money,
                        ReservationDate = reservation.ReservationDate.ToString("yyyy-MM-dd"),
                        ReservationStatus = reservation.IsBooked ? 0 : (now >= reservation.StartTime ? 0 : reservation.ReservationStatus),
                        IsBooked = reservation.IsBooked,
                        SpaceId = reservation.SpaceId.ToString(),
                        SpaceType = reservation.SpaceType,
                        SpaceName = reservation.SpaceName

                    });
                }
                tag.InitStatus(true);
            }
            mobileResModel.Status = true;
            mobileResModel.Data = tag;
            return mobileResModel;
        }

        [HttpGet("Info/{id:long}")]
        public async ValueTask<MobileResModel> ReservationInfo(long id) 
        {
            MobileResModel mobileResModel = new MobileResModel();
            var model= await _reservationRepository.ReservationInfo(id);
            if (model == null) 
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该预约场地不存在";
                return mobileResModel;
            }
            if (model.IsBooked|| model.ReservationStatus == 0)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该场地已预约";
                return mobileResModel;
            }
            if (model.StartTime<=DateTime.Now)
            {
                mobileResModel.Status = false;
                mobileResModel.Msg = "该场地预约时间已结束";
                return mobileResModel;
            }
            mobileResModel.Status = true;
            mobileResModel.Data = new
            {
                model.ReservationId,
                model.ReservationDate,
                model.SpaceName,
                ReservationTime = model.StartTime.ToString("HH:ss") + "~" + model.EndTime.ToString("HH:ss"),
                model.Money

            };
            return mobileResModel;
        }

    }
}
