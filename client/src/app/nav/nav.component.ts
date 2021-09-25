import { renderFlagCheckIfStmt } from '@angular/compiler/src/render3/view/template';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../ClientServices/account.service';
import { Observable } from 'rxjs';
import {User} from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any ={}
  
  constructor(public accountService:AccountService, private router: Router
    , private toastr:ToastrService) { }

  ngOnInit(): void
  { }

  //Login method
  login()
  {

    this.accountService.login(this.model).subscribe(response => {
     //Route to page by the url
      this.router.navigateByUrl('/members');

    }, error => 
    {

      console.log(error);
      this.toastr.error(error.error);

    });

  }

  logout()
  {

    this.accountService.logout();
    //Route to page by the url
    this.router.navigateByUrl('/');
  
  }


}
