

namespace SmartCityWebApi.ViewModels
{
    public record LoginUserViewModel
    {
        public string UserName { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
