namespace SmartCityWebApi.Domain.IRepository
{
    public interface IReservationRepository
    {
        ValueTask<IEnumerable<dynamic>> GetReservationList(DateOnly date);
    }
}
