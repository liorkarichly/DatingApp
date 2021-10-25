using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {

        private readonly DataContext r_Context;
        private readonly IMapper r_Mapper;

        //Repository injection to database
        public UserRepository(DataContext i_Context, IMapper i_Mapper)
        {

            r_Mapper = i_Mapper;
            r_Context = i_Context;

        }

        public async Task<MemberDTOs> GetMemberAsync(string username)
        {

            return await r_Context.Users.Where(//We show in termnial the property of AppUser
                member => member.UserName == username)
            .ProjectTo<MemberDTOs>(r_Mapper.ConfigurationProvider)//Set configuration file for to provide our alternative profile, we need to provider the our configuration that the mapper known to access    
            .SingleOrDefaultAsync();//execute query

        }

        // public async Task<IEnumerable<MemberDTOs>> GetMembersAsync()
        // {
            
        //     return await r_Context
        //                 .Users
        //                 .ProjectTo<MemberDTOs>(r_mapper.ConfigurationProvider)
        //                 .ToListAsync();
            
            
        // }

        public async Task<PagedList<MemberDTOs>> GetMembersAsync(UserParams userParams)
        {
            
            var query =  r_Context.Users.AsQueryable();
                        // .ProjectTo<MemberDTOs>(r_mapper.ConfigurationProvider)
                        // .AsNoTracking()
                        // .AsQueryable();
            query = query.Where(user => user.UserName != userParams.CurrentUsername);
            query = query.Where(user => user.Gender == userParams.Gender);
            var minDateOfBirth =  DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDateOfBirth =  DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(userAge => userAge.DateOfBirth >= minDateOfBirth
                                && userAge.DateOfBirth <= maxDateOfBirth);

            query =  userParams.OrederBy switch{//switch exspression (version 8.0+)

                "created" =>  query.OrderByDescending(userCreate => userCreate.Created),
                /*Default*/_ => query.OrderByDescending(userLastActive => userLastActive.LastActive)
           
            };
                                //Still send query but first we do filter and still not executing anything inside this method because that's still being taken care of inside
            return await PagedList<MemberDTOs>.CreateAsync(query
                                                         .ProjectTo<MemberDTOs>(r_Mapper.ConfigurationProvider) 
                                                         .AsNoTracking()//We're not going to do anything with these entities.
                                                         , userParams.PageNumber
                                                         , userParams.PageSize);
            
            
        }

        /**
       Return user by id
       */
        public async Task<AppUser> GetUserByIdAsync(int id)
        {

            return await r_Context.Users.FindAsync(id);

        }

        /**
        Return user by username
        */
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {

            return await r_Context.Users
                    .Include(photosUser => photosUser.Photos)//Include photos of user
                    .SingleOrDefaultAsync(
                        user => user.UserName == username);//Return by username, execute query
            

            // .Include - //Take related collection, ,include with our response but its make a problme beacuse its do a cycle exceptions
            // When we allow to photo object so we have them ApppUser object and it access them
            //And its make a cycle calls

        }


        /**
        Return list of users
        */
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {

            return await r_Context.Users
                .Include(photosUser => photosUser.Photos)
                .ToListAsync();

        }

        /**
       Save changes  of context by user
       Use in UnitOfWork
       */
        // public async Task<bool> SaveAllAsAsync()
        // {

        //     return await r_Context.SaveChangesAsync() > 0;

        // }

        public void Update(AppUser user)
        {

            //When we canges the property or state of user so we need to UPDATE that
            //Flag
            r_Context.Entry(user).State = EntityState.Modified;

        }

        
        public async Task<string> GetUserGender(string username)
        {
           
           return await r_Context.Users
                .Where(user => user.UserName == username)
                .Select(user => user.Gender)
                .FirstOrDefaultAsync();
        
        }
    }
}