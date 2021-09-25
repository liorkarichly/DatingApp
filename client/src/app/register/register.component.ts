import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../ClientServices/account.service';
                //Input Decorator
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() usersFromHomeComponent: any;//Property
  @Output() cancelRegister = new EventEmitter(); //Property
  model:any = {};


  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  register()
  {
    
    this.accountService.register(this.model).subscribe(response =>
      {

        console.log(response);//Get from map from register component 
        this.cancel();

      }, error =>
      {

        console.log(error);
        this.toastr.error(error.error.title);

      });
  }

  cancel(){
   this.cancelRegister.emit(false);
  }
}
