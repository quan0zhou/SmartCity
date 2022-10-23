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
        public string ReservationTitle { get; set; } = default!;

        [Column(TypeName = "time")]
        public TimeOnly StartTime { get; set; }

        [Column(TypeName = "time")]
        public TimeOnly EndTime { get; set; }

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



        [Column(TypeName = "varchar(200)")]
        public string AppID { get; set; } = default!;

        [Column(TypeName = "varchar(200)")]
        public string MchID { get; set; } = default!;

        [Column(TypeName = "varchar(200)")]
        public string SubMchID { get; set; } = default!;

        [Column(TypeName = "varchar(200)")]
        public string AppKey { get; set; } = default!;

        [Column(TypeName = "varchar(200)")]
        public string AppSecret { get; set; } = default!;

        /// <summary>
        /// 微信商户证书序列号
        /// </summary>
        [Column(TypeName = "varchar(200)")]
        public string CertificateSerialNumber { get; set; } = default!;

        /// <summary>
        /// 微信商户证书私钥
        /// </summary>
        [Column(TypeName = "varchar(2048)")]
        public string CertificatePrivateKey { get; set; } = default!;

        public long CreateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
    }
}
