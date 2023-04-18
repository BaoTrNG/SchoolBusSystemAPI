using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolBusApi.Models
{
    public class parent
    {
        [Key]
        public int parent_id { get; set; }
        public string name { get; set; }
        public string? email { get; set; }

        public string phone { get; set; }
        public string? user_id { get; set; }

    }
}
