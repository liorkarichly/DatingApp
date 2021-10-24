import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageService } from 'src/app/ClientServices/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  
  @ViewChild('messageForm')messageForm: NgForm;
  @Input()username: string;//We get username name that we click on them for get list of messages with him
  @Input ()messages:Message[];
  messageContent: string;
   
 
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

    this.messageService.sendMessage(this.username, this.messageContent).then(
      message =>
      {

        //this.messages.push(message);
        this.messageForm.reset();
      
      });

  }

}
