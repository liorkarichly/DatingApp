using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {

        private readonly DataContext r_DataContext;
        private readonly IMapper r_Mapper;

        public MessageRepository(DataContext i_DataContext, IMapper i_Mapper)
        {

            this.r_DataContext = i_DataContext;
            this.r_Mapper = i_Mapper;

        }

      
        public void AddMessage(Message message)
        {

            r_DataContext.Messages.Add(message);

        }

        public void DeleteMassage(Message message)
        {

            r_DataContext.Messages.Remove(message);

        }

      
        public async Task<Message> GetMessage(int id)
        {

            return await r_DataContext.Messages
            .Include(user => user.Sender)
            .Include(user => user.Recipient)
            .SingleOrDefaultAsync(message => message.Id == id);

        }

        public async Task<PagedList<MessageDTOs>> GetMessagesForUser(MessageParams messageParams)
        {

            var query = r_DataContext.Messages
            .OrderByDescending(message => message.MessageSent)
            .ProjectTo<MessageDTOs>(r_Mapper.ConfigurationProvider)
            .AsQueryable();//Return messages that return

            query = messageParams.Container switch
            {

                "Inbox" => query.Where(user => user.RecipientUsername == messageParams.Usernmae 
                && user.RecipientDeleted == false),
                "Outbox" => query.Where(user => user.SenderUsername == messageParams.Usernmae
                 && user.SenderDeleted == false),
                _ => query.Where(user => user.RecipientUsername== messageParams.Usernmae && user.DateRead == null
                && user.RecipientDeleted == false)

            };

            //I Projection this in var query 
            //var messages = query.ProjectTo<MessageDTOs>(r_Mapper.ConfigurationProvider);

            return await PagedList<MessageDTOs>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDTOs>> GetMessageThread(string currentUsername
                                                             , string recipientUsername)
        {
            
            /*This is big query and i want to improve it!*/
            var messages = await  r_DataContext.Messages
                            // .Include(user => user.Sender).ThenInclude(photo => photo.Photos)
                            // .Include(user => user.Recipient).ThenInclude(photo => photo.Photos)//We can down this because tha we have the projection and i want only the message
                            .Where(message => message.Recipient.UserName == currentUsername
                                    && message.RecipientDeleted == false
                                    && message.Sender.UserName == recipientUsername
                                    || message.Recipient.UserName == recipientUsername
                                    && message.Sender.UserName == currentUsername
                                    && message.SenderDeleted == false)
                            .OrderBy(message => message.MessageSent)
                            .ProjectTo<MessageDTOs>(r_Mapper.ConfigurationProvider)//Mapper before that we sending to the list
                            .ToListAsync();//Get converstion of users

            var unreadMessages = messages//pull unread Messages
            .Where(message => message.DateRead == null && message.RecipientUsername == currentUsername).ToList();

                if(unreadMessages.Any())
                {

                    foreach(var message in messages)
                    {

                        message.DateRead = DateTime.UtcNow;//Mark them as read

                    }

                    //await r_DataContext.SaveChangesAsync();//Pass to MessageHub

                }

            return messages;
                
        }

        /*
        Save about messages
        Use in UnitOfWork
        */
        // public async Task<bool> SaveAllAsync()
        // {

        //     return await r_DataContext.SaveChangesAsync() > 0;

        // }

        /*------------------Manaer Group Chats --------------------*/

        public void AddGroup(Group group)
        {
            
            r_DataContext.Groups.Add(group);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
           
           return await r_DataContext.Connections.FindAsync(connectionId);

        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            
            return await r_DataContext.Groups
            .Include(groupConnection => groupConnection.Connection)
            .FirstOrDefaultAsync(groupConnection => groupConnection.Name == groupName);

        }

        public void RemoveConnection(Connection connection)
        {
            
            r_DataContext.Connections.Remove(connection);
            
        }

        public async Task<Group> GetGroupForConnections(string connectionId)
        {
            
            return await r_DataContext.Groups
            .Include(connection => connection.Connection)
            .Where(connection => connection.Connection.Any(user => user.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        
        }
    }
}