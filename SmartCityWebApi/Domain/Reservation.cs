namespace SmartCityWebApi.Domain
{
    public class Reservation
    {
        public long ReservationId { get; set; }

        public DateOnly ReservationDate { get; set; }

        public long SpaceId { get; set; }

        public int SpaceType { get; set; }

        public string SpaceName { get; set; } = string.Empty;
    }
}
