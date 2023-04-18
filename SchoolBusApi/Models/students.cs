using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SchoolBusApi.Models
{
    public class students
    {
        [Key]
        public string student_id { get; set; }

        public string student_name { get; set; }

        public int parent_id { get; set; }
        public bool active { get; set; }

        public Guid registry_id { get; set; }

    }
}
