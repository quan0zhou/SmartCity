using IdGen;
using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain.IRepository;

namespace SmartCityWebApi.Infrastructure.Repository
{
    public class ReservationRepository: IReservationRepository
    {
        private readonly SmartCityContext _smartCityContext;
        private readonly IdGenerator _idGenerator;
        public ReservationRepository(SmartCityContext smartCityContext, IdGenerator idGenerator)
        {
            _smartCityContext = smartCityContext;
            _idGenerator = idGenerator;
        }

        public async ValueTask<IEnumerable<dynamic>> GetReservationList(DateOnly date) 
        {
        
             return await _smartCityContext.Reservations.AsNoTracking().Where(r=>r.ReservationDate>=date).OrderBy(r=>r.ReservationDate).ThenBy(r=>r.SpaceName).Select(r=>new 
              {
                 ReservationId= r.ReservationId.ToString(),
                 r.SpaceName,
                 r.ReservationStatus,
                 ReservationDate=r.ReservationDate.ToString("yyyy-MM-dd"),
                 r.Money,
                 StartTime=r.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                 EndTime=r.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
              
              }).ToListAsync();


        }

    }
}
