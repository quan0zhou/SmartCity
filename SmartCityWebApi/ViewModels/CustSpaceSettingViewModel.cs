using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.ViewModels
{
    public record CustSpaceSettingViewModel
    {
        public long? CustId { get; set; }


        public string ReservationTitle { get; set; } = default!;


        public TimeOnly StartTime { get; set; }


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



        public string AppID { get; set; } = default!;


        public string MchID { get; set; } = default!;


        public string SubMchID { get; set; } = default!;


        public string AppKey { get; set; } = default!;

        public string AppSecret { get; set; } = default!;

    }
}
