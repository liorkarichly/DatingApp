using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub: Hub
    {

        //private readonly IMessageRepository r_MessageRepository;
        //private readonly IUserRepository r_UserRepository;
        private readonly IMapper r_Mapper;
        
        private readonly IHubContext<PresenceHub> r_PresenceHub;
        private readonly PresenceTracker r_PresenceTracker;

        private readonly IUnitOfWork r_UnitOfWork;

        public MessageHub(//IMessageRepository i_MessageRepository
                        // , IUserRepository i_UserRepository
                        IUnitOfWork i_UnitOfWork
                        ,IMapper i_Mapper              
                        ,IHubContext<PresenceHub> i_PresenceHub
                        ,PresenceTracker i_PresenceTracker)
        {

            //this.r_MessageRepository = i_MessageRepository;
            // this.r_UserRepository = i_UserRepository;
            this.r_Mapper = i_Mapper;     
            this.r_PresenceHub = i_PresenceHub;
            this.r_PresenceTracker = i_PresenceTracker;
            this.r_UnitOfWork = i_UnitOfWork;

        }


        //Create of group each member
        public override async Task OnConnectedAsync()
        {
            
            var httpContext = Context.GetHttpContext();//Get hold of the of a user's username
            var otherUser = httpContext.Request.Query["user"].ToString();//Add property or a varibale for the other user and allow with query
            /*when we make a connection to this hub, we're going to pass in the other user name with the key of user.
            And get this into this particular property, we need to know which user profile the currently logged in user has clicked on and we can get that via a query string that we can use when we create this particular hub connection.*/

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);//Get group

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);//Add to group

            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);//Send the group name to group

            // var messages = await r_MessageRepository Use in UnitOfWork
            //         .GetMessageThread(Context.User.GetUsername(), otherUser);//Create the message to users on group

            var messages = await r_UnitOfWork.MessageRepository
                    .GetMessageThread(Context.User.GetUsername(), otherUser);

            if(r_UnitOfWork.HasChanges())
            {

                await r_UnitOfWork.Complete();
                
            }
            //await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);//Send the messages 
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }


        /*Each pair of users in a conversation is going to have their
         own group so that only they will be able to 
         receive the message that the messages that they send 
         to each other.*/
        private string GetGroupName(string i_Caller, string i_Other)
        {

                //This in alphabetical order
                var stringComapre = string.CompareOrdinal(i_Caller, i_Other) < 0;

                return stringComapre ? $"{i_Caller}-{i_Other}":$"{i_Other}-{i_Caller}";
        
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

        /* Passingly the exception now signal when a user connects and disconnects and they are a 
           member of the group , then when they disconnect is automatically going to 
           remove them from that particular group.
        */

            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("Updategroup", group);
            await  base.OnDisconnectedAsync(exception);

        }

        public async Task SendMessage(CreateMessageDTOs createMessageDto)
        {

            
            var username = Context.User.GetUsername();//Take username of sender

            if(username == createMessageDto.RecipientUsername.ToLower())//Checking if the username sender to him self
            {

                throw new HubException("You cannot send message yourself");

            }

            // var sender = await r_UserRepository.GetUserByUsernameAsync(username);//Take the user
            // var recipient = await r_UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            var sender = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(username);//Take the user
            var recipient = await r_UnitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);


            if(recipient == null)
            {

                throw new HubException("Not found user");

            }

            var message = new Message
            {

                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecupientUsername = recipient.UserName,
                Content = createMessageDto.Content

            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            //var group = await r_MessageRepository.GetMessageGroup(groupName);
            var group = await r_UnitOfWork.MessageRepository.GetMessageGroup(groupName);

            if(group.Connection.Any(groups => groups.Username == recipient.UserName))//Only when the users is connection
            {
                
                message.DateRead = DateTime.UtcNow;
           
            }
            else//When the user is not connection but i want toshow him that he get message, we see the message when we in text box message and i want to show the message when we flow in app
            {

                var connections = await r_PresenceTracker.GetConnectionsForUser(recipient.UserName);

                if(connections != null)
                {

                    await r_PresenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived"
                                , new {username = sender.UserName, knownAs = sender.KnownAs});

                }
                
            }

            //r_MessageRepository.AddMessage(message);
            r_UnitOfWork.MessageRepository.AddMessage(message);

            //if(await r_MessageRepository.SaveAllAsync())
            if(await r_UnitOfWork.Complete())
            {

              
                //return Ok(r_Mapper.Map<MessageDTOs>(message));//Mapper my massage 
                await Clients.Group(groupName).SendAsync("NewMessage", r_Mapper.Map<MessageDTOs>(message));
            
            }

        }


        /* Adding and Removing a user from a group and say who is connection*/
        public async Task<Group> AddToGroup(string groupName)
        {

            //var group = await r_MessageRepository.GetMessageGroup(groupName);//Get group name
            var group = await r_UnitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());//Create new connextion when the user connnection

            if(group == null)//If its new group
            {

                group = new Group(groupName);
                //r_MessageRepository.AddGroup(group);
                r_UnitOfWork.MessageRepository.AddGroup(group);

            }

            group.Connection.Add(connection);//The user in group get new id every connection 

            //if( await r_MessageRepository.SaveAllAsync())
            if( await r_UnitOfWork.Complete())
            {

                return group;

            }

            throw new HubException("failed to join group");

        }

        private async Task<Group> RemoveFromMessageGroup()
        {
                                                            //We inside the hub so we can to use in Context
            //var group = await r_MessageRepository.GetGroupForConnections(Context.ConnectionId);
           var group = await r_UnitOfWork.MessageRepository.GetGroupForConnections(Context.ConnectionId);
            var connection = group.Connection.FirstOrDefault(user => user.ConnectionId == Context.ConnectionId);
           
            // r_MessageRepository.RemoveConnection(connection);  
            r_UnitOfWork.MessageRepository.RemoveConnection(connection);

            // if(await r_MessageRepository.SaveAllAsync())
            if(await r_UnitOfWork.Complete())
            {

                return group;

            }

            throw new HubException("Failed to remove from group");
            
        }

    }

}