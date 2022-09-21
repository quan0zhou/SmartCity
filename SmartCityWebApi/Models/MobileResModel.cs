namespace SmartCityWebApi.Models
{
    public class MobileResModel
    {
        public bool Status { get; set; }

        public string Msg { get; set; } = default!;

        public dynamic? Data { get; set; } = default!;
    }
}
