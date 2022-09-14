namespace SmartCityWebApi.ViewModels
{
    public record ReservationItemViewModel
    {
        public long ReservationId { get; set; } 
    }

    public record ReservationListItemViewModel
    {
        public ReservationItemViewModel[] Items { get; set; } = default!;

        public decimal Money { get; set; }
    }
}
