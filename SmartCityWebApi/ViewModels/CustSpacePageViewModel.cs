namespace SmartCityWebApi.ViewModels
{
    public record CustSpacePageViewModel:BasePageViewModel
    {

        public int? SpaceType { get; set; }

        public string? SpaceName { get; set; }

        public string? ContactName { get; set; } 
    }
}
