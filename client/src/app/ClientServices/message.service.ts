import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
baseUrl = environment.apiUrl;
hubUrl = environment.hubUrl;
private hubConnection: HubConnection;
private messageThreadSource = new BehaviorSubject<Message[]>([]);
messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http:HttpClient
            , private busyService: BusyService) { }

  getMessages(pageNumber, pageSize, container)
  {

    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('Container', container);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);

  }

  getMessageThread(username:string)
  {

    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content:string)//Return promsie,anything that's decorated with async we can use await operators inside here if we wished
  {

    //return this.http.post<Message>(this.baseUrl + 'messages', {recipientUsername: username,  content})
  
    //We use in hub for call to api and send message, and we return the hub connection and use in 'invoke' that using the specified name and arguments
    return this.hubConnection.invoke('SendMessage',  {recipientUsername: username,  content})
            .catch(error => console.log(error));

  }

  deleteMessage(id:number)
  {

    return this.http.delete(this.baseUrl + 'messages/' + id);
    
  }

  createHubConnection(user: User, otherUsername: string)
  {
    
    this.busyService.busy();

    this.hubConnection = new HubConnectionBuilder()
                            .withUrl(this.hubUrl + 'message?user=' + otherUsername, 
                            {

                              accessTokenFactory: () => user.token
                            
                            })
                            .withAutomaticReconnect()
                            .build();

    this.hubConnection.start()
    .catch(error => console.log(error))
    .finally(() => this.busyService.idle());
     
    this.hubConnection.on('ReceiveMessageThread', messages =>//Color in red when i dont read the message
    {

      this.messageThreadSource.next(messages);

    });

    this.hubConnection.on('NewMessage', message =>
    {

      this.messageThread$.pipe(take(1)).subscribe(messages =>
      {

        this.messageThreadSource.next([...messages, message]);

      });

    });

    /**we want to do is take a look inside our message thread and see if there's any
     *  unread messages for the user, let's just join this group, and if there are
     * ,then we're going to mark them as read inside here. */
    this.hubConnection.on('UpdaedGroup', (group: Group) =>
    {

      if(group.connections.some(connection => connection.username === otherUsername))
      {

        this.messageThread$.pipe(take(1)).subscribe(messages => 
          {
            /**When i upload the conversation of users so i checking what is read and what is unread and when the 
             * user enter to chat box so is make that the read message
             */
            messages.forEach(message => {

              if(!message.dateRead)
              {

                message.dateRead = new Date(Date.now());

              }

            });

            this.messageThreadSource.next([...messages]);//Concat the messages

          });

      }


    });

  }

  stopConnection()
  {

    if(this.hubConnection)
    {
      
      this.messageThreadSource.next([]);
      this.hubConnection.stop();

    }

  }

  
}
