using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolBusApi.Models
{
    public class ServiceRegistry
    {
        [Key]
        public Guid id { get; set; }

        public string? stop_id { get; set; }

        public string? car_id { get; set; }
        public string? staff_id { get; set; }

        public bool is_cancel { get; set; }

    }
}
