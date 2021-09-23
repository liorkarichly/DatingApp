import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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


  constructor(private accountService: AccountService) { }

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
        
      });
  }

  cancel(){
   this.cancelRegister.emit(false);
  }
}
