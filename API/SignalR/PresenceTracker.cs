using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {

                                        //<Username connection, List of the connection IDs>
        private static readonly Dictionary<string, List<string>> sr_OnlineUser = 
                                new Dictionary<string, List<string>>();
        
        public Task<bool> UserConnected(string username, string cnnectionId)
        {

                bool isOnline = false;
                /*We use in lock because that weill use many users
                that want to get updates.
                And this dictionary is going to be shared amongst everyone who connects to our server and the dictionary is not a threat to safe resource.
                So if we had concurrent users trying to update this at the same time, then we're probably going to run into problems.*/
                lock(sr_OnlineUser)
                {

                    if(sr_OnlineUser.ContainsKey(username))
                    {

                        sr_OnlineUser[username].Add(cnnectionId);

                    }
                    else
                    {

                        sr_OnlineUser.Add(username, new List<string>{cnnectionId});
                        isOnline = true;

                    }

                }

            return Task.FromResult(isOnline);

        }

        public Task<bool> UserDisconnected(string username, string disconnectedId)
        {

            bool isOffline = false;
            lock(sr_OnlineUser)
            {

                if(!sr_OnlineUser.ContainsKey(username))
                {
                
                    return Task.FromResult(isOffline);

                }

                sr_OnlineUser[username].Remove(disconnectedId);

                if(sr_OnlineUser[username].Count == 0)
                {

                    sr_OnlineUser.Remove(username);
                    isOffline = true;

                }

            }

            return Task.FromResult(isOffline);

        }

        public Task<string[]> GetOnlineUsers()
        {

            string[] onlineUsers;
            lock(sr_OnlineUser)
            {

                onlineUsers = sr_OnlineUser//Get from memory and not database
                            .OrderBy(keyUser => keyUser.Key)//Who Connect or not
                            .Select(keyUser => keyUser.Key)//Take the connection users
                            .ToArray();
            }

            return Task.FromResult(onlineUsers);

        }
    
        public Task<List<string>> GetConnectionsForUser(string username)
        {

            List<string> connectionIds;
            lock(sr_OnlineUser)
            {

                connectionIds = sr_OnlineUser.GetValueOrDefault(username);//Return connection ids of user
           
            }

            return Task.FromResult(connectionIds);//Return the list connections

        }
    
    }
    
}