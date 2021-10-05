using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Collections.Generic;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using API.interfaces;
using API.DTOs;
using AutoMapper;

namespace API.Controllers
{

    // [ApiController]
    // [Route("api/[controller]")]//api-start//[controller]-placeholder
    //[Authorize]
    public class UsersController : BaseApiController
    {

        // private readonly DataContext r_DataContext;
        private readonly IUserRepository r_UserRepository;
        private readonly IMapper r_Mapper;

        public UsersController(IUserRepository i_UserRepository, IMapper i_Mapper) //:base(i_Context)
        {
            r_Mapper = i_Mapper;
            r_UserRepository = i_UserRepository;

            // r_DataContext = i_Context;

        }

        //End point for list users, result to client
        //Async Task its make to call to database in synchornous
        // [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTOs>>> GetUsers()
        {

            //Ok() magically formats it to a  OkResult and OkResult inherits from ActionResult.
            //Return table.nameTable.List
            //await - Operator that wait to call to API for to work on background thread
            //return Ok(await r_UserRepository.GetUsersAsync());
            // OR 
            //return r_DataContext.Users.ToListAsync().result; -- not after that  Ok(await r_UserRepository.GetUsersAsync()); its version better


            var users = await r_UserRepository.GetUsersAsync();
            var usersToReturn = r_Mapper.Map<IEnumerable<MemberDTOs>>(users);
                                                           //TO

            return Ok(usersToReturn);
        }

        // //api/users/3
        // // [Authorize]
        // [HttpGet("{Id}")]
        // public async Task<ActionResult<AppUser>> GetUsers(int Id)///Endpoint for specifics parameter
        // {

        //     ///Return the user by id , if is exists in database
        //     return await r_UserRepository.GetUserByIdAsync(Id);

        // }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTOs>> GetUsers(string username)///Endpoint for specifics parameter
        {


            ///Return the user by id , if is exists in database
            //return await r_UserRepository.GetUserByUsernameAsync(username);

            var user = await r_UserRepository.GetUserByUsernameAsync(username);
            return r_Mapper.Map<MemberDTOs>(user);
        }
        //I remove the [Authorize] and [AllowAnonymous] because that i want to make to user 
        // to get the other users when they authentacion
    }

}