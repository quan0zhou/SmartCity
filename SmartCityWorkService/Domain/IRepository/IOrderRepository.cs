using SmartCityWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCityWorkService.Domain.IRepository
{
    public interface  IOrderRepository
    {
        ValueTask<IList<Order>> GetPendingCancelOrders();
        ValueTask<bool> CancelOrders(long[] orderIds, long[] reservationIds);
    }
}
