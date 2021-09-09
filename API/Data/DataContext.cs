using Microsoft.EntityFrameworkCore;
using API.Entities;

namespace API.Data
{

    public class DataContext: DbContext
    {
        
        //Injection point DbContext
        public DataContext(DbContextOptions i_Options)
        : base(i_Options)
        { }

        //Class to set in database
        public DbSet<AppUser> Users { get; set; }

    }

}