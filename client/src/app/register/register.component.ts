import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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
  registerForm: FormGroup
  maxDate: Date;
  validationErrors: string[] = [];

//FormBuilder - Called the form builder service and will inject the form builder inside here.
  constructor(private accountService: AccountService
            , private toastr: ToastrService
            ,private formBuilder: FormBuilder//Create AbstractControl, Create Syntax that shortens creating instans FormGroup, FormControl
            ,private router: Router) { }

  ngOnInit(): void {

    this.intiailizeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);//The user must be a 18 years (Minimum)

  }

  register()
  {
    
    //console.log(this.registerForm.value);
    this.accountService.register(this.registerForm.value).subscribe(response =>
    {

       // console.log(response);//Get from map from register component 
        this.router.navigateByUrl('/members');//After that the user finish to register so we router her to mambers page

    },  error =>
    {

        console.log(error);
        //this.toastr.error(error.error.title);
        this.validationErrors = error;
        
    });

  }

  cancel()
  {

   this.cancelRegister.emit(false);

  }

  //Create group control in register page and validation
  intiailizeForm()
  {

    // //Options 1
    // this.registerForm = new FormGroup(
    //   {

    //     username: new FormControl('', Validators.required),
    //     password: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(10)]),
    //     confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])

    //   }

    // );

    // this.registerForm.controls.password.valueChanges.subscribe(() =>
    // {

    //   this.registerForm.controls.confirmPassword.updateValueAndValidity();

    // });

    //Options 2
      this.registerForm = this.formBuilder.group(
      {
        //this validator only goes one way

        username: ['', Validators.required],
        gender: ['male'],//Button chose, default
        knownAs: ['', Validators.required],
        dateOfBirth: ['', Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required], 
        password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(10)]],
        confirmPassword: ['', [Validators.required, this.matchValues('password')]]

      }

    );
      // the validator on the confirmPassword field again when the password field is updated. 
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });

  }

  matchValues(matchTo: string): ValidatorFn
  {

    //Back from AbstractControl beacuse every our control from him
    return (control:AbstractControl) =>
    {
      //Compare between the password and confirm password
      return control?.value == control?.parent?.controls[matchTo].value? null: {isMatching: true}
      
    }

  }

}
