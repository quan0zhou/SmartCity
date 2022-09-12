using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using SmartCityWorkService.Domain.IRepository;

namespace SmartCityWorkService.Infrastructure.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly SmartCityContext _smartCityContext;

        public ReservationRepository(SmartCityContext smartCityContext)
        {
            _smartCityContext = smartCityContext;
        }

        public async ValueTask<bool> DeleteReservation(DateOnly date)
        {
           return await _smartCityContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM \"reservation\" WHERE \"ReservationDate\"<={date}")>0;
        }

        public async ValueTask<IEnumerable<CustSpace>> GetCustSpaceList()
        {
            return await _smartCityContext.CustSpaces.AsNoTracking().OrderBy(r => r.SpaceName).ToListAsync();
        }

        public async ValueTask<CustSpaceSetting?> GetCustSpaceSetting()
        {
            return await _smartCityContext.CustSpaceSettings.AsNoTracking().FirstOrDefaultAsync();
        }

        public async ValueTask<bool> ReservationSave(IEnumerable<Reservation> reservations) 
        {
            _smartCityContext.Reservations.AddRange(reservations);
            return await _smartCityContext.SaveChangesAsync() > 0;
        }

        public async ValueTask<int> GetReservationCount(DateOnly date)
        {
            return await _smartCityContext.Reservations.AsNoTracking().GroupBy(r=>r.ReservationDate).Where(r => r.Key > date).CountAsync();
        }
    }
}
