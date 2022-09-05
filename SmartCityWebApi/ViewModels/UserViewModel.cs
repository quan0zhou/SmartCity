using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.ViewModels
{
    public record UserViewModel
    {
        public long? UserId { get; set; }

        public string UserName { get; set; } = default!;

        public string UserAccount { get; set; } = default!;

        public string? UserAccountPwd { get; set; }

        public string ContactPhone { get; set; } = default!;

        public string Remark { get; set; } = default!;

        public List<int> Pers { get; set; } = default!;

    }
}
