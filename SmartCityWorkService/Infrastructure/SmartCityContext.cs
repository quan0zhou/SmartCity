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

        public DbSet<CustSpace> CustSpaces => Set<CustSpace>();
        public DbSet<CustSpaceSetting> CustSpaceSettings => Set<CustSpaceSetting>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
