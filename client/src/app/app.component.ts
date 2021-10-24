import { Component, OnInit } from '@angular/core';
import { AccountService } from './ClientServices/account.service';
import { PresenceService } from './ClientServices/presence.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Dating App';
  users: any;

  constructor(private accountService:AccountService,
            private presenceService: PresenceService)
  {}

  ngOnInit()
  {

  this.setCurrentUser();

  }

  setCurrentUser()
  {

    const user: User = JSON.parse(localStorage.getItem('user'));

    if(user)
    {

      this.accountService.setCurrentUser(user);
      this.presenceService.createHubConnection(user);

    }

  }

}