using System.ComponentModel.DataAnnotations;

namespace SchoolBusApi.Models
{
    public class BusRoutes
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);

        [Key]
        public string route_id { get; set; }
        public string route_name { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }

    }
}
