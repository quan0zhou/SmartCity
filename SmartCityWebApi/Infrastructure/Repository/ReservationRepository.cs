using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Domain.IRepository;

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

        public async ValueTask<bool> SetReservationMoney(long[] ids, decimal money)
        {
            return await _smartCityContext.Database.ExecuteSqlRawAsync($"UPDATE \"reservation\" SET \"Money\"={{0}},\"ReservationStatus\"=1 WHERE \"ReservationId\" IN ({(string.Join(',', ids))}) AND \"IsBooked\"=FALSE", money) > 0;
        }

        public async ValueTask<bool> SetReservationStatus(long[] ids, bool isUnreservable)
        {
            return await _smartCityContext.Database.ExecuteSqlRawAsync($"UPDATE \"reservation\" SET \"ReservationStatus\"={{0}} WHERE \"ReservationId\" IN ({(string.Join(',',ids))}) AND \"IsBooked\"=FALSE", (isUnreservable ? 0 : 1)) > 0;
        }
    }
}
