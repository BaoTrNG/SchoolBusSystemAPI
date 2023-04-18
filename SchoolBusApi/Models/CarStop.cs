using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SchoolBusApi.Models
{
    public class CarStop
    {
        [Key]
        public string stop_id { get; set; }
        public string stop_name { get; set; }

        [Precision(10, 8)]
        public decimal latitude { get; set; }
        [Precision(10, 8)]
        public decimal longitude { get; set; }
        public string number { get; set; }
        public string street { get; set; }
        public string? ward { get; set; } = null;
        public string? district { get; set; } = null;
        public string? city { get; set; } = null;
        public double range_from_school { get; set; }
        public double cost { get; set; }
        public int student_count { get; set; }
        public int staff_id { get; set; }

    }
}
