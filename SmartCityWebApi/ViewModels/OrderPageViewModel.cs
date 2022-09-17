namespace SmartCityWebApi.ViewModels
{
    public record OrderPageViewModel : BasePageViewModel
    {
        public long? SpaceId { get; set; }

        public int? SpaceType { get; set; }

        public int? Status { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? StartTime { get; set; }

        public int? EndTime { get; set; }

        public string? UserName { get; set; }

        public string? UserPhone { get; set; }

    }

}
