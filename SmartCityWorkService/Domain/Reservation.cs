using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.Domain
{
    [Table("reservation")]
    public class Reservation
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ReservationId { get; set; }

        [Column(TypeName = "date")]
        public DateOnly ReservationDate { get; set; }

        public long SpaceId { get; set; }

        public int SpaceType { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string SpaceName { get; set; } = default!;

        [Column(TypeName = "timestamp")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 【0：不可用 1：可用】
        /// </summary>
        public int ReservationStatus { get; set; }


        public decimal Money { get; set; }
    }
}
