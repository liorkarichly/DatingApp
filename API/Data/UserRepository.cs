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
        private readonly IMapper r_mapper;

        //Repository injection to database
        public UserRepository(DataContext i_Context, IMapper i_mapper)
        {
            r_mapper = i_mapper;

            r_Context = i_Context;

        }

        public async Task<MemberDTOs> GetMemberAsync(string username)
        {

            return await r_Context.Users.Where(//We show in termnial the property of AppUser
                member => member.UserName == username)
            .ProjectTo<MemberDTOs>(r_mapper.ConfigurationProvider)//Set configuration file for to provide our alternative profile, we need to provider the our configuration that the mapper known to access    
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

            query =  userParams.OrederBy switch{

                "created" =>  query.OrderByDescending(userCreate => userCreate.Created),
                /*Default*/_ => query.OrderByDescending(userLastActive => userLastActive.LastActive)
           
            };

            return await PagedList<MemberDTOs>.CreateAsync(query
                                                         .ProjectTo<MemberDTOs>(r_mapper.ConfigurationProvider) 
                                                         .AsNoTracking()
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
                        user => user.UserName == username);//Return by username
            

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
       */
        public async Task<bool> SaveAllAsAsync()
        {

            return await r_Context.SaveChangesAsync() > 0;

        }

        public void Update(AppUser user)
        {

            //When we canges the property or state of user so we need to UPDATE that
            //Flag
            r_Context.Entry(user).State = EntityState.Modified;

        }
    }
}