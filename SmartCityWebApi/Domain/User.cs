using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("user")]
    public class User
    {
        [Key]
        public long UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserAccount { get; set; } = string.Empty;

        public string UserAccountPwd { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string Remark { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
