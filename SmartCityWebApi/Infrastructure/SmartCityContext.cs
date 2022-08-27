using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SmartCityWebApi.Domain;
using SmartCityWebApi.Extensions;
using System.Reflection.Metadata;

namespace SmartCityWebApi.Infrastructure
{
    public class SmartCityContext:DbContext
    {

        public SmartCityContext(DbContextOptions<SmartCityContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<CustSpace> CustSpaces { get; set; }
        public DbSet<CustSpaceSetting> CustSpaceSettings { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}
