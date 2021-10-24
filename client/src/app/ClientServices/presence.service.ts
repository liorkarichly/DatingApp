import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})

export class PresenceService {
  
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;//Type of HubConnection that we get from SIGNALER and what we're also doing here is will inject our taste to service
  private onlineUsersSource = new BehaviorSubject<string[]>([]);//The subject is a generic observable and this one requires an initial value and emits its current value whenever it is subscribed to.
                                                  //Array of username 
  
  onlineUsers$ = this.onlineUsersSource.asObservable();//Going to be unobservable and we'll say this to online.
 
 constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User)//We want the token of user when he enter
  //We cannot use our JWT interceptor. These are no longer going to be HTTP requests, these are going to be different..
  {

    this.hubConnection = new HubConnectionBuilder().withUrl(
                              // Returns a string containing the access token.
    this.hubUrl + 'presence', {accessTokenFactory: () => user.token})
    //If there's a network problem, our client is automatically going to try and reconnect to our hub.
    .withAutomaticReconnect()
    .build();

    this.hubConnection.start()
    .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => 
    {
      
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        
        this.onlineUsersSource.next([...usernames, username]);//Add username to list of usernames that connections
    
      });//Say the user is connect
   
    });

    this.hubConnection.on('UserIsOffline', username => //UserIsOffline must to wirte exatcly like in API server
    {
      
     this.onlineUsers$.pipe(take(1)).subscribe(usernames =>
      {

        this.onlineUsersSource.next([...usernames.filter(user => user !== username)]);

      });//Say the user is disconnect
   
    });

    //reate an ever listening event and we can say this dothub connection on.
    //And we can say get online users.
    this.hubConnection.on('GetOnlineUsers', (usernames:string[]) =>
    {

      this.onlineUsersSource.next(usernames);

    });
    
    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) =>
     {

      this.toastr.info(knownAs + 'has sent you a new message!')//When i calick on the toastr so we navigate to message inbox
      .onTap
      .pipe(take(1))
      .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));

    });

  }


  stopHubConnection()
  {

    this.hubConnection.stop().catch(error => console.log(error));
    
  }

}
