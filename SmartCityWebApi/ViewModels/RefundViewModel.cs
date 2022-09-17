namespace SmartCityWebApi.ViewModels
{
    public record RefundViewModel
    {

        public long OrderId { get; set; }

        public string Remark { get; set; } = default!;
    }
}
