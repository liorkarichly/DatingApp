using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]//No anonymous users would be able to access or connect to our particular hub here.
    //we want this to be only for our authorized users.
    public class PresenceHub: Hub
    {
        private readonly PresenceTracker r_PresenceTracker;

        public PresenceHub(PresenceTracker i_PresenceTracker)
        {

            this.r_PresenceTracker = i_PresenceTracker;

        }
        
        //Override from class Hub that tell me when the user is conncet to hub 
        public override async Task OnConnectedAsync()
        {

                /*
                    optimal solution:
                  we have no way of knowing who is inside the group, 
                  any one given time just using a hub.
                  And the reason is same as the reason for not being 
                  able to do this with the online presence is that if 
                  we had more than one server, then signal hours has
                  got no way of knowing if a user's connected to a 
                  different server.
                  it was really messy and it was tricky code to go and
                  get the connected users out of there.
                  And it just became a spagetti nonsense of mess.
                  So what we'll do instead this time and to give us
                  a look at a different way of tracking users in
                  groups is we'll use our database.
                  the optimal solution I've mentioned before is to not 
                  do this in a database because the database is 
                  persistent storage, whereas something like Redus 
                  operates in memory on different servers and it could 
                  be distributed across different servers as well.

                   
                    best solution:
                  Use in database for control about connection users
                   */

           var isOnline = await r_PresenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            if(isOnline)//Checking if the user is online and return only if the user is online
            {

                //Client connect to hub and we get access to clients, any clients get the message without the user that send him
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());


            }
            

            /*When a client connects, we're going to update our presence tracker and we're going to send the updated*/
            var currentUsers = await r_PresenceTracker.GetOnlineUsers();
            ///await Clients.All.SendAsync("GetOnlineUsers", currentUsers); To any users
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers); 
        
        }

         //Override from class Hub that tell me when the user is disconncet from hub 
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var isOffline =await r_PresenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            
            if(isOffline)
            {

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            }

            /*When a client connects, we're going to update our presence tracker and we're going to send the updated*/
            // var currentUsers = await r_PresenceTracker.GetOnlineUsers(); I dont to notification when the user is disconnected
            // await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
          
            await base.OnDisconnectedAsync(exception);

        }

    }
}