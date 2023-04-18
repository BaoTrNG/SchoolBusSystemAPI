using System.ComponentModel.DataAnnotations;

namespace SchoolBusApi.Models
{
    public class car
    {
        [Key]
        public string id { get; set; }
        public string serial { get; set; }
        public int driver_id { get; set; }
        public int slot { get; set; }
        public int remain_slot { get; set; }

        public Guid tracker_id { get; set; }

    }
}
