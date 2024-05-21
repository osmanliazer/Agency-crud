using Agency.Models;
using Microsoft.EntityFrameworkCore;

namespace Agency.DAL
{
    public class AgencyContext:DbContext
    {
        public AgencyContext(DbContextOptions<AgencyContext> options):base(options) 
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
