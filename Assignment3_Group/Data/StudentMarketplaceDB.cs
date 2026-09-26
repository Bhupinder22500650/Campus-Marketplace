using Microsoft.EntityFrameworkCore;
using Assignment3_Group.Models;

namespace Assignment3_Group.Data
{
    public class StudentMarketplaceDB:DbContext
    {
        public StudentMarketplaceDB(DbContextOptions<StudentMarketplaceDB> options) : base(options) 
        { 
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Listings> Listings { get; set; }
    }
}
