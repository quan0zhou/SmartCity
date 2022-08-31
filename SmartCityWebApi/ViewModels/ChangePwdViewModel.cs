namespace SmartCityWebApi.ViewModels
{
    public record ChangePwdViewModel
    {
        public string OldPassword { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string ConfirmPassword { get; set; } = default!;
    }
}
