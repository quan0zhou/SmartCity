

namespace SmartCityWebApi.Models
{
    public class ReservationTag
    {
        public string Date { get; set; } = default!;

        public string Week { get; set; } = default!;


        public int Status { get; set; } = default!;


        public List<ReservationItem> Items { get; set; } = new List<ReservationItem>();

        public string[] SpaceArray { get; set; } = default!;

        public dynamic[] TimeArray { get; set; } = default!;

        public void InitStatus(bool isLimitTime)
        {

            if (isLimitTime)
            {
                this.Status = this.Items.Where(r => r.StartTime > DateTime.Now && r.ReservationStatus == 1 && r.IsBooked == false && this.Date.Equals(r.ReservationDate)).Any() ? 1 : 0;
            }
            else
            {
                this.Status = this.Items.Where(r => r.ReservationStatus == 1 && r.IsBooked == false && this.Date.Equals(r.ReservationDate)).Any() ? 1 : 0;
            }

            this.SpaceArray = this.Items.GroupBy(r => r.SpaceName).OrderBy(r => r.Key).Select(r => r.Key).ToArray();
            this.TimeArray = this.Items.GroupBy(r => new { r.StartTime, r.EndTime }).OrderBy(r => r.Key.StartTime).Select(r => new
            {
                r.Key.StartTime,
                r.Key.EndTime,
                StartTimeStr = r.Key.StartTime.ToString("HH:mm"),
                EndTimeStr = r.Key.EndTime.ToString("HH:mm")
            }).ToArray();
        }


    }


    public class ReservationItem
    {
        public string ReservationId { get; set; } = default!;
        public string ReservationDate { get; set; } = default!;

        public int SpaceType { get; set; }
        public string SpaceId { get; set; } = default!;

        public string SpaceName { get; set; } = default!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 【0：不可用 1：可用】
        /// </summary>
        public int ReservationStatus { get; set; }



        public bool IsBooked { get; set; }


        public decimal Money { get; set; }

    }
}
