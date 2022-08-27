using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; set; }


        [Column(TypeName = "varchar(50)")]
        public string UserName { get; set; } = string.Empty;

        [Column(TypeName = "varchar(20)")]
        public string UserAccount { get; set; } = string.Empty;

        [Column(TypeName = "varchar(500)")]
        public string UserAccountPwd { get; set; } = string.Empty;

        [Column(TypeName = "varchar(20)")]
        public string ContactPhone { get; set; } = string.Empty;

        [Column(TypeName = "varchar(500)")]
        public string Remark { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public long CreateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
    }
}
