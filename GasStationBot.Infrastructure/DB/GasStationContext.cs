using GasStationBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GasStationBot.Infrastructure.DB
{
    public class GasStationContext : DbContext
    {
        public GasStationContext(DbContextOptions<GasStationContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().Property(u => u.GasStations)
                .HasConversion(
                    gs => JsonConvert.SerializeObject(gs),
                    gs => JsonConvert.DeserializeObject<List<UserGasStation>>(gs));
        }
    }
}
