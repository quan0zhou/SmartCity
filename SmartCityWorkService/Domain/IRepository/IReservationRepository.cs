

using SmartCityWebApi.Domain;

namespace SmartCityWorkService.Domain.IRepository
{
    public interface IReservationRepository
    {

        ValueTask<IEnumerable<CustSpace>> GetCustSpaceList();
        ValueTask<CustSpaceSetting?> GetCustSpaceSetting();

        ValueTask<bool> DeleteReservation(DateOnly date);

        ValueTask<bool> ReservationSave(IEnumerable<Reservation> reservations);

        ValueTask<int> GetReservationCount(DateOnly date);



    }
}
