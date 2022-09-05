using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string OrderNo { get; set; } = default!;


        [Column(TypeName = "varchar(50)")]
        public string PaymentNo { get; set; } = default!;

        [Column(TypeName = "varchar(200)")]
        public string OpenId { get; set; } = default!;

        [Column(TypeName = "varchar(20)")]
        public string ReservationUserName { get; set; } = default!;

        [Column(TypeName = "varchar(20)")]
        public string ReservationUserPhone { get; set; } = default!;

        public long ReservationId { get; set; }

        [Column(TypeName = "date")]
        public DateOnly ReservationDate { get; set; }

        public long SpaceId { get; set; }

        public int SpaceType { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string SpaceName { get; set; } = default!;

        [Column(TypeName = "timestamp")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime EndTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 订单状态（0：约定待确认 1：已预定 2：已退款 3：退款待确认 4：拒绝退款）
        /// </summary>
        public int OrderStatus { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime RefundTime { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string RefundRemark { get; set; } = default!;

        [Column(TypeName = "varchar(100)")]
        public string RefundOptUser { get; set; } = default!;
    }
}
