namespace SmartCityWebApi.Models
{
    public class Setting
    {
        public string NotifyUrl { get; set; } = default!;
        public string LimitWeekName { get; set; } = default!;
        public string LimitTimeOfDay { get; set; } = default!;
    }
}
