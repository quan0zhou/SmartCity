﻿namespace SmartCityWebApi.Domain.IRepository
{
    public interface IReservationRepository
    {
        ValueTask<IEnumerable<Reservation>> GetReservationList(DateOnly date,bool isEqual,DateOnly? endDate=null);

        ValueTask<bool> SetReservationStatus(long[] ids, bool isUnreservable);

        ValueTask<bool> SetReservationMoney(long[] ids, decimal money);
    }
}
