using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SchoolBusApi.Models
{
    public class bill
    {
        [Key]
        public Guid bill_id { get; set; }
        public string? transaction_id { get; set; }
        public DateTime create_time { get; set; }
        public DateTime end_time { get; set; }
        public int days_used { get; set; }
        public DateTime? paid_time { get; set; }
        public bool is_pay { get; set; }
        public bool is_cancel { get; set; }
        public int parent_id { get; set; }
        public string student_id { get; set; }
        public string stop_id { get; set; }
        public double cost { get; set; }
        public int? staff_id { get; set; }
    }
}
