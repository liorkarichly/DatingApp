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
            .AsQueryable();//Return messages that return

            query = messageParams.Container switch
            {

                "Inbox" => query.Where(user => user.Recipient.UserName == messageParams.Usernmae 
                && user.RecipientDeleted == false),
                "Outbox" => query.Where(user => user.Sender.UserName == messageParams.Usernmae
                 && user.SenderDeleted == false),
                _ => query.Where(user => user.Recipient.UserName == messageParams.Usernmae && user.DateRead == null
                && user.RecipientDeleted == false)

            };


            var messages = query.ProjectTo<MessageDTOs>(r_Mapper.ConfigurationProvider);

            return await PagedList<MessageDTOs>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDTOs>> GetMessageThread(string currentUsername
                                                             , string recipientUsername)
        {
            
            var messages = await  r_DataContext.Messages
                            .Include(user => user.Sender).ThenInclude(photo => photo.Photos)
                            .Include(user => user.Recipient).ThenInclude(photo => photo.Photos)
                            .Where(message => message.Recipient.UserName == currentUsername
                                    && message.RecipientDeleted == false
                                    && message.Sender.UserName == recipientUsername
                                    || message.Recipient.UserName == recipientUsername
                                    && message.Sender.UserName == currentUsername
                                    && message.SenderDeleted == false)
                            .OrderBy(message => message.MessageSent)
                            .ToListAsync();//Get converstion of users

            var unreadMessages = messages//Unread Messages
            .Where(message => message.DateRead == null && message.Recipient.UserName == currentUsername).ToList();

                if(unreadMessages.Any())
                {

                    foreach(var message in messages)
                    {

                        message.DateRead = DateTime.Now;//Mark them as read

                    }

                    await r_DataContext.SaveChangesAsync();

                }

            return r_Mapper.Map<IEnumerable<MessageDTOs>>(messages);//Return MessageDTOs
                
        }

        public async Task<bool> SaveAllAsync()
        {


            return await r_DataContext.SaveChangesAsync() > 0;

        }
    }
}