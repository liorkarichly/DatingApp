using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
    public interface IMessageRepository
    {

        //Menager our group with the chat
        void AddGroup(Group group);

        void RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);

        Task<Group> GetMessageGroup(string groupName);

        Task<Group> GetGroupForConnections(string connectionId);

        //Manager message

        void AddMessage(Message message);

        void DeleteMassage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDTOs>> GetMessagesForUser(MessageParams messageParams);/*we want to give the user the opportunity to see
         their inbox outbox and unread messages inside here,and we also 
         are returning a page list, which means we need to delete the responses as well*/

        Task<IEnumerable<MessageDTOs>> GetMessageThread(string currentUsername, string recipientUsername);

       //Task<bool> SaveAllAsync();Use in UnitOfWork
        

    }
}