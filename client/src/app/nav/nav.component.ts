import { renderFlagCheckIfStmt } from '@angular/compiler/src/render3/view/template';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../ClientServices/account.service';
import { Observable } from 'rxjs';
import {User} from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any ={}
  
  constructor(public accountService:AccountService) { }

  ngOnInit(): void
  { }

  //Login method
  login()
  {

    this.accountService.login(this.model).subscribe(response => {
     
    }, error => 
    {

      console.log(error);
      
    });

  }

  logout()
  {

    this.accountService.logout();
  
  }


}
