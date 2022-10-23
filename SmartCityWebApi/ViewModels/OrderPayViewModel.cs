namespace SmartCityWebApi.ViewModels
{
    public record OrderPayViewModel
    {
        public long ReservationId { get; set; }

        public string OpenId { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string UserPhone { get; set; } = default!;

    }
}
