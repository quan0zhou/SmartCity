using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("custSpace")]
    public class CustSpace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SpaceId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string SpaceName { get; set; } = default!;

        [Column(TypeName = "varchar(200)")]
        public string SpaceAddress { get; set; } = default!;

        [Column(TypeName = "varchar(20)")]
        public string ContactName { get; set; } = default!;


        [Column(TypeName = "varchar(20)")]
        public string ContactPhone { get; set; } = default!;


        [Column(TypeName = "varchar(500)")]
        public string Remark { get; set; } = default!;

        /// <summary>
        /// 场地类型【1:网球场 2：排球场 3：篮球场 4：羽毛球场】
        /// </summary>
        public int SpaceType { get; set; }

        public long CreateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }

    }
}
