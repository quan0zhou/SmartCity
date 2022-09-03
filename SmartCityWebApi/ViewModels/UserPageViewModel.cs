namespace SmartCityWebApi.ViewModels
{
    public record UserPageViewModel:BasePageViewModel
    {
        public string? Keyword { get; set; }
    }
}
