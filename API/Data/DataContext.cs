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
        
        //Create table of likes
        public DbSet<UserLike> Likes { get; set; }

        //Gives to entities some configuration, create relationships many to many
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

                base.OnModelCreating(modelBuilder);//we add try and add immigration


                //Use in our entities 'USerLike', and create key that can to use with source key
                //and we have the key to table
                modelBuilder.Entity<UserLike>()
                .HasKey(key => new {key.SourceUserId, key.LikedUserId});


                //Create to relationship 
                modelBuilder.Entity<UserLike>()
                .HasOne(source => source.SourceUser)//One user, key
                .WithMany(iLikeHim => iLikeHim.LikedUsers)//Like many, key
                .HasForeignKey(sourceKey => sourceKey.SourceUserId)//Set source id
                .OnDelete(DeleteBehavior.Cascade);//If we delete user so we delete the related users

            /* Importent:
            
                if you use in SQL Server so you need to set the DeleteBehavior.Cascade
                here to 'DeleteBehavior.NoAction'... or will get an error 
                during migration
            */
        //Other side of the relationship 
                modelBuilder.Entity<UserLike>()
                .HasOne(source => source.LikedUser)//One user, key
                .WithMany(iLikeHim => iLikeHim.LikedByUsers)//Like many, key
                .HasForeignKey(sourceKey => sourceKey.LikedUserId)//Set source id
                .OnDelete(DeleteBehavior.Cascade);//If we delete user so we delete the related users
        
        }
        
    }

}