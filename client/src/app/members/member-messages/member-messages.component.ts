import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageService } from 'src/app/ClientServices/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush, //change detection strategy and override its defaults and its default is to check always.
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  
  @ViewChild('messageForm')messageForm: NgForm;
  @Input()username: string;//We get username name that we click on them for get list of messages with him
  @Input ()messages:Message[];
  messageContent: string;
  loading = false;
   
 
  constructor(public messageService:MessageService) { }

  ngOnInit(): void {
  //this.loadMessages();//Pass to member-details.component
  }

  // loadMessages()
  // {

  //   this.messageService.getMessageThread(this.username).subscribe
  //   (response => {
  //     this.messages = response;
  //   });

  // }

  sendMessage()
  {

    this.loading = true;
    this.messageService.sendMessage(this.username, this.messageContent).then(
      message =>
      {

        //this.messages.push(message);
        this.messageForm.reset();
      
      })
      .finally(() => this.loading = false);

  }

}
