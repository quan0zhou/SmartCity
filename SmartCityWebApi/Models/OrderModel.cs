using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;

namespace SmartCityWebApi.Models
{
    [ExcelExporter(TableStyle = OfficeOpenXml.Table.TableStyles.Light13)]
    public class OrderModel
    {
        [ExporterHeader(DisplayName = "最后操作时间")]
        public string UpdateTime { get; set; } = default!;

        [ExporterHeader(DisplayName = "状态")]
        public string OrderStatusName { get; set; } = default!;

        [ExporterHeader(DisplayName = "场地类型")]
        public string SpaceTypeName { get; set; } = default!;

        [ExporterHeader(DisplayName = "场地")]
        public string SpaceName { get; set; } = default!;

        [ExporterHeader(DisplayName = "预定日期")]
        public string ReservationDate { get; set; } = default!;


        [ExporterHeader(DisplayName = "时间段")]
        public string ReservationTime { get; set; } = default!;

        [ExporterHeader(DisplayName = "预订金额")]
        public decimal Money { get; set; }

        [ExporterHeader(DisplayName = "预订人姓名")]
        public string ReservationUserName { get; set; } = default!;

        [ExporterHeader(DisplayName = "预订人手机号")]
        public string ReservationUserPhone { get; set; } = default!;

        [ExporterHeader(DisplayName = "微信OpenId")]
        public string OpenId { get; set; } = default!;

        [ExporterHeader(DisplayName = "支付时间")]
        public string PayTime { get; set; } = default!;

        [ExporterHeader(DisplayName = "商户订单号")]
        public string OrderNo { get; set; } = default!;

        [ExporterHeader(DisplayName = "交易订单号")]
        public string PaymentNo { get; set; } = default!;


        [ExporterHeader(DisplayName = "退款(或拒绝)时间")]
        public string RefundTime { get; set; } = default!;


        [ExporterHeader(DisplayName = "退款(或拒绝)操作人")]
        public string RefundOptUser { get; set; } = default!;

        [ExporterHeader(DisplayName = "退款(或拒绝)备注")]
        public string RefundRemark { get; set; } = default!;

        [ExporterHeader(DisplayName = "订单创建时间")]
        public string CreateTime { get; set; } = default!;


    }
}
