using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("custSpaceSetting")]
    public class CustSpaceSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CustId { get; set; }


        [Column(TypeName = "varchar(20)")]
        public string ReservationTitle { get; set; } = string.Empty;

        [Column(TypeName = "timestamp")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime EndTime { get; set; }

        public float TimePeriod { get; set; }

        /// <summary>
        /// 可设置天数
        /// </summary>
        public int SettableDays { get; set; }

        /// <summary>
        /// 可预订天数
        /// </summary>
        public int BookableDays { get; set; }

        /// <summary>
        /// 多少小时之外可以直接退，否则需要审核退款
        /// </summary>
        public float DirectRefundPeriod { get; set; }

        /// <summary>
        /// 距离预订时间多长后，不可退款
        /// </summary>
        public float NoRefundPeriod { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string AppID { get; set; } = string.Empty;

        [Column(TypeName = "varchar(200)")]
        public string MchID { get; set; } = string.Empty;

        [Column(TypeName = "varchar(200)")]
        public string SubMchID { get; set; } = string.Empty;

        [Column(TypeName = "varchar(200)")]
        public string AppKey { get; set; } = string.Empty;

        [Column(TypeName = "varchar(200)")]
        public string AppSecret { get; set; } = string.Empty;

        public long CreateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
    }
}
