import { Component, OnInit } from '@angular/core';
import { ConfirmService } from '../ClientServices/confirm.service';
import { MessageService } from '../ClientServices/message.service';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
messages: Message[];
pagination: Pagination
container = 'Unread';
pageNumber = 1;
pageSize = 10;
loading = false;

  constructor(private messageService: MessageService
    , private confirmService: ConfirmService) { }

  ngOnInit(): void 
  {

    this.loadMessages();

  }

    loadMessages()
    {

      this.loading = true;//When the Messages reload to componnet
      this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(
        response =>
        {

        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
        });

    }

    pageChanged(event: any)
    {

      this.pageNumber = event.page;
      this.loadMessages();

    }

    deleteMessage(id: number)
    {
      
      this.confirmService.confirm('Confirm delete message', 'This cannot be undone')
      .subscribe(result =>
        {
          if(result)
          {

            this.messageService.deleteMessage(id).subscribe
            (() => 
            {
      
              this.messages
              .splice(this.messages.findIndex(messageDelete => messageDelete.id === id), 1);
      
            });
      

          }
        })
     
    }

}
