import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
baseUr = environment.apiUrl;

  constructor(private http:HttpClient) { }

  getMessages(pageNumber, pageSize, container)
  {

    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('Container', container);

    return getPaginatedResult<Message[]>(this.baseUr + 'messages', params, this.http);

  }

  getMessageThread(username:string)
  {

    return this.http.get<Message[]>(this.baseUr + 'messages/thread/' + username);
  }

  sendMessage(username: string, content:string)
  {

    return this.http.post<Message>(this.baseUr + 'messages', {recipientUsername: username,  content})
  
  }

  deleteMessage(id:number)
  {

    return this.http.delete(this.baseUr + 'messages/' + id);
    
  }

  
}
