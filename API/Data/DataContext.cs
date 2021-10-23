using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{

    public class DataContext: IdentityDbContext<AppUser, AppRole, int
    , IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>
    , IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        
        //Injection point DbContext
        public DataContext(DbContextOptions i_Options)
        : base(i_Options)
        { }

        //We have IdentityDbContext
        //Class to set in database
        //public DbSet<AppUser> Users { get; set; }
        
        //Create table of likes
        public DbSet<UserLike> Likes { get; set; }

        //Create table of Messages
        public DbSet<Message> Messages { get; set; }

        //Gives to entities some configuration, create relationships many to many
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

                base.OnModelCreating(modelBuilder);//we add try and add immigration

                modelBuilder.Entity<AppUser>()
                .HasMany(userRole => userRole.UserRoles)
                .WithOne(user => user.User)
                .HasForeignKey(userRole => userRole.UserId)
                .IsRequired();

                modelBuilder.Entity<AppRole>()
                .HasMany(userRole => userRole.UserRoles)
                .WithOne(user => user.Role)
                .HasForeignKey(userRole => userRole.RoleId)
                .IsRequired();

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


                modelBuilder.Entity<Message>()
                    .HasOne(user => user.Recipient)//One recipient
                    .WithMany(member => member.MessagesReceived)//Collection of recipient
                    .OnDelete(DeleteBehavior.Restrict);//We don't want to remove the messages if the other party.
                                                        //Hasn't deleted them themselves.
        
                modelBuilder.Entity<Message>()
                    .HasOne(user => user.Sender)//One sender
                    .WithMany(member => member.MessagesSent)//Collection of sent
                    .OnDelete(DeleteBehavior.Restrict);

        }
        
    }

}