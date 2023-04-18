using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolBusApi.Models
{
    public class users
    {
        [Key]
        public int user_id { get; set; }
        public string username { get; set; }
        public string pass { get; set; }
        public string accountType { get; set; }
    }
}
