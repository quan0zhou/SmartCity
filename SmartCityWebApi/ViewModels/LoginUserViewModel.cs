

namespace SmartCityWebApi.ViewModels
{
    public record LoginUserViewModel
    {
        public string UserName { get; init; } = default!;

        public string Password { get; init; } = default!;
    }
}
