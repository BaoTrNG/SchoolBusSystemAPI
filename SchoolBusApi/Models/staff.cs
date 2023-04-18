using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolBusApi.Models
{
    public class staff
    {
        [Key]
        public int staff_id { get; set; }
        public string staff_name { get; set; }
        public string cccd { get; set; }

        public string phone { get; set; }
        public string email { get; set; }
        public int user_id { get; set; }
        public int role_id { get; set; }

    }
}
