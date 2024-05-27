using IdGen;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;
using SmartCityWebApi.Extensions;
using SmartCityWebApi.Models;
using System.Linq.Dynamic.Core;

namespace SmartCityWebApi.Infrastructure.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly SmartCityContext _smartCityContext;
        private readonly IdGenerator _idGenerator;
        public ReservationRepository(SmartCityContext smartCityContext, IdGenerator idGenerator)
        {
            _smartCityContext = smartCityContext;
            _idGenerator = idGenerator;
        }

        public DateTime GetMaxDate(DateTime now)
        {
            DateTime endDate;
            if (now.ToWeekName() == "星期一")
            {
                if (now >= Convert.ToDateTime("09:00"))
                {
                    endDate = now.AddDays(7);
                }
                else
                {
                    endDate = now;
                }

            }
            else
            {
                int week = (int)now.DayOfWeek;
                endDate = now.AddDays(7 - (week == 0 ? 7 : week) + 1);
            }
            return endDate;
        }

        public async ValueTask<IEnumerable<Reservation>> GetReservationList(DateOnly date, bool isEqual, DateOnly? endDate = null)
        {
            var query = _smartCityContext.Reservations.AsNoTracking();
            if (isEqual)
            {
                query = query.Where(r => r.ReservationDate == date);
            }
            else
            {
                query = query.Where(r => r.ReservationDate >= date);
            }
            if (endDate.HasValue)
            {
                query = query.Where(r => r.ReservationDate <= endDate.Value);
            }
            return await query.ToListAsync();
        }

        public async ValueTask<ReservationItem?> ReservationInfo(long id)
        {
            return await _smartCityContext.Reservations.Where(r => r.ReservationId.Equals(id)).Select(r => new ReservationItem
            {
                ReservationId = r.ReservationId.ToString(),
                ReservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ReservationStatus=r.ReservationStatus,
                IsBooked=r.IsBooked,
                Money=r.Money,
                SpaceId=r.SpaceId.ToString(),
                SpaceName=r.SpaceName,
                SpaceType=r.SpaceType
            }).FirstOrDefaultAsync();
        }

        public async ValueTask<bool> SetReservationMoney(long[] ids, decimal money)
        {
            return await _smartCityContext.Database.ExecuteSqlRawAsync($"UPDATE \"reservation\" SET \"Money\"={{0}},\"ReservationStatus\"=1 WHERE \"ReservationId\" IN ({(string.Join(',', ids))}) AND \"IsBooked\"=FALSE", money) > 0;
        }

        public async ValueTask<bool> SetReservationStatus(long[] ids, bool isUnreservable)
        {
            return await _smartCityContext.Database.ExecuteSqlRawAsync($"UPDATE \"reservation\" SET \"ReservationStatus\"={{0}} WHERE \"ReservationId\" IN ({(string.Join(',', ids))}) AND \"IsBooked\"=FALSE", (isUnreservable ? 0 : 1)) > 0;
        }
    }
}
