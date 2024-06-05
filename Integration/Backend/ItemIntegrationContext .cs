using Integration.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Backend
{
    public class ItemIntegrationContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;port=3308;database=integrationApp;user=root;password=Erdem1998", new MySqlServerVersion(new Version(8, 3, 0)));
        }
        
        public DbSet<Item> Items => Set<Item>();
    }
}
