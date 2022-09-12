using Microsoft.EntityFrameworkCore;
using SmartCityWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCityWorkService.Infrastructure
{
    public class SmartCityContext : DbContext
    {

        public SmartCityContext(DbContextOptions<SmartCityContext> options) : base(options)
        {

        }

        public DbSet<CustSpace> CustSpaces { get; set; }
        public DbSet<CustSpaceSetting> CustSpaceSettings { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
