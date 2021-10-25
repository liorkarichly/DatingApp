using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {

        //Use in UnitOfWork
        // private readonly IUserRepository r_UserRepository;
        // private readonly IMessageRepository r_MessageRepository;
         

        // public MessagesController(IUserRepository i_UserRepository
        //                         , IMessageRepository i_MessageRepository,
        //                         IMapper i_Mapper)
        // {

        //     this.r_UserRepository = i_UserRepository;
        //     this.r_MessageRepository = i_MessageRepository;
        //     this.r_Mapper = i_Mapper;
        // }

        private readonly IUnitOfWork r_UnitOfWork;
        private readonly IMapper r_Mapper;


        public MessagesController(IUnitOfWork i_UnitOfWork, IMapper i_Mapper)
        {

            this.r_UnitOfWork = i_UnitOfWork;

        }


        /*PASS TO MESSAGE HUB*/
        // [HttpPost]
        // public async Task<ActionResult<MessageDTOs>> CreateMessage(CreateMessageDTOs createMessage)
        // {
            
        //     
        //     var username = User.GetUsername();//Take username of sender

        //     if(username == createMessage.RecipientUsername.ToLower())//Checking if the username sender to him self
        //     {

        //         return BadRequest("You cannot send message yourself");

        //     }

        //     var sender = await r_UserRepository.GetUserByUsernameAsync(username);//Take the user
        //     var recipient = await r_UserRepository.GetUserByUsernameAsync(createMessage.RecipientUsername);

        //     if(recipient == null)
        //     {

        //         return NotFound();

        //     }

        //     var message = new Message
        //     {

        //         Sender = sender,
        //         Recipient = recipient,
        //         SenderUsername = sender.UserName,
        //         RecupientUsername = recipient.UserName,
        //         Content = createMessage.Content

        //     };

        //     r_MessageRepository.AddMessage(message);

        //     if(await r_MessageRepository.SaveAllAsync())
        //     {

        //         return Ok(r_Mapper.Map<MessageDTOs>(message));//Mapper my massage 

        //     }

        //         return BadRequest("Failed to send message");

        // }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTOs>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            
            messageParams.Usernmae = User.GetUsername();

            //var messages = await r_MessageRepository.GetMessagesForUser(messageParams);
            var messages = await r_UnitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize
            , messages.TotalCount, messages.TotalPages);

            return messages;
            
        }

        //Use in SignalR (Hub) /*PASS TO MESSAGE HUB*/
        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDTOs>>> GetMessageThread(string username)
        // {

        //         var currentUsername = User.GetUsername();

        //         return Ok(await r_MessageRepository.GetMessageThread(currentUsername, username));
        //         

        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {

            var username = User.GetUsername();
            //var message =  await r_MessageRepository.GetMessage(id);
            var message =  await r_UnitOfWork.MessageRepository.GetMessage(id);

            /*The sender don't use the name.
            Is not equal to username and the message recipients.*/
            if(message.Sender.UserName != username 
            && message.Recipient.UserName != username)
            {

                return Unauthorized();

            } 

            if(message.Sender.UserName == username)
            {

                message.SenderDeleted = true;

            }

            if(message.Recipient.UserName == username)
            {
                
                message.RecipientDeleted = true;

            }

            if(message.SenderDeleted && message.RecipientDeleted)
            {

                 //Use in UnitOfWork
                //r_MessageRepository.DeleteMassage(message);
                r_UnitOfWork.MessageRepository.DeleteMassage(message);

            }

            //Use in UnitOfWork
            // if(await r_MessageRepository.SaveAllAsync())
            // {

            //     return Ok();
            
            // }

              if(await r_UnitOfWork.Complete())
            {

                return Ok();
            
            }

            return BadRequest("Problem deleting the message");
            
        }

    }
    
}