namespace SmartCityWebApi.Domain.Enum
{
    public enum OrderStatusEnum
    {
        /// <summary>
        /// 预订待支付
        /// </summary>
        ReservationPendingPayment=0,
        /// <summary>
        /// 已预订
        /// </summary>
        Booked=1,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded=2,

        /// <summary>
        /// 待退款
        /// </summary>
        RefundPending = 3,
        /// <summary>
        /// 拒绝退款
        /// </summary>
        RefusalToRefund =4
    }
}
