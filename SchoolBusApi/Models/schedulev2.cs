using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolBusApi.Models
{
    public class schedulev2
    {
        public string car_id { get; set; }
        public string stop_id { get; set; }
        public string pickup_time { get; set; }

        public string drop_time { get; set; }
        public int staff_id { get; set; }
    }
}
