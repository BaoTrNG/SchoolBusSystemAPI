using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using SchoolBusApi.Models;
using System.Data;

namespace SchoolBusApi.data
{
    public class BusService : DbContext
    {
        public string ConnectionString { get; set; }
        public BusService(DbContextOptions<BusService> options) : base(options)
        {
            ConnectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
        }
        public DbSet<students> students { get; set; }
        public DbSet<car> buses { get; set; }

        public Task<List<car>> GetAllBus() => buses.ToListAsync();
    }
}
