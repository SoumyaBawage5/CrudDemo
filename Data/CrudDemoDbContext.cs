using CrudDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudDemo.Data
{
    public class CrudDemoDbContext : DbContext
    {
        public CrudDemoDbContext(DbContextOptions options) : base(options)
        {
           

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            optionsBuilder.EnableSensitiveDataLogging();

           
        }

        public DbSet<Contact> Contacts { get; set; }
  
       

    }


}
