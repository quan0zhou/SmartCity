using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("custSpace")]
    public class CustSpace
    {
        [Key]
        public long SpaceId { get; set; }

        public string SpaceName { get; set; } = string.Empty;

        public string SpaceAddress { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 场地类型【1:网球场 2：排球场 3：篮球场 4：羽毛球场】
        /// </summary>
        public int SpaceType { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateTime { get; set; }

    }
}
