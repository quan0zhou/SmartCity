using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("userPermission")]
    public class UserPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserPermissionId { get; set; }

        public long UserId { get; set; }

        public int PageId { get; set; }
    }
}
