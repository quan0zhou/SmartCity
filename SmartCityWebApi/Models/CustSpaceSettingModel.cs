namespace SmartCityWebApi.Models
{
    public class CustSpaceSettingModel
    {
        public string CustId { get; set; } = default!;

        public string ReservationTitle { get; set; } = default!;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public float TimePeriod { get; set; }

        public int SettableDays { get; set; }

        public int BookableDays { get; set; }

        public float DirectRefundPeriod { get; set; }

        public string AppID { get; set; } = default!;
        public string MchID { get; set; } = default!;
        public string SubMchID { get; set; } = default!;

        public string AppKey { get; set; } = default!;

        public string AppSecret { get; set; } = default!;

        public string CertificatePrivateKey { get; set; } = default!;

        public string CertificateSerialNumber { get; set; } = default!;

    }
}
