using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Options;

using SchoolBusApi.Models;
namespace SchoolBusApi.data
{
    public class RouteService : DbContext
    {
        public string ConnectionString { get; set; }
        public RouteService(DbContextOptions<RouteService> options) : base(options)
        {
            ConnectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
        }

    }
}
