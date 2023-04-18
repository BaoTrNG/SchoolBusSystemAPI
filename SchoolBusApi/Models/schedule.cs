using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolBusApi.Models
{
    public class schedule
    {
        [Key]
        public string car_id { get; set; }
        public string stop_id { get; set; }
        public TimeSpan pickup_time { get; set; }

        public TimeSpan drop_time { get; set; }
        public int staff_id { get; set; }

    }
}
