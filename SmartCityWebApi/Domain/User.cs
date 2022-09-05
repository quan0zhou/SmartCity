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
        public string UserName { get; set; } = default!;

        [Column(TypeName = "varchar(20)")]
        public string UserAccount { get; set; } = default!;

        [Column(TypeName = "varchar(500)")]
        public string UserAccountPwd { get; set; } = default!;

        [Column(TypeName = "varchar(20)")]
        public string ContactPhone { get; set; } = default!;

        [Column(TypeName = "varchar(500)")]
        public string Remark { get; set; } = default!;

        public bool IsAdmin { get; set; }

        public long CreateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
    }
}
