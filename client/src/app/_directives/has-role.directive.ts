import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { AccountService } from '../ClientServices/account.service';
import { User } from '../_models/user';

@Directive({
  selector: '[appHasRole]' /**WE use in *nngIf, *ngFor... and bsRadio...
                              */
                  /**WE want to pass array of parameters of roles of the user use */
})
export class HasRoleDirective implements OnInit {
@Input()appHasRole: string[];
user:User;

  constructor(private viewContainerRef: ViewContainerRef,
              private templateRef: TemplateRef<any>,
              private accountService: AccountService )
  { 

    this.accountService.currentUser$.pipe(take(1)).subscribe(
      user => {

        this.user = user;//Get access to our user

      }
    )
  }
  ngOnInit(): void {
   
    //Clear view if no roles
   if(!this.user.roles || this.user == null)
   {

    this.viewContainerRef.clear();
    return;

   }

   if(this.user?.roles.some(role => this.appHasRole.includes(role)))//return true if on have one of the element in array (roles) and check in appHasRole
   {

    this.viewContainerRef.createEmbeddedView(this.templateRef);//Open the role in nav bar

   }
   else
   {

    this.viewContainerRef.clear();

   }
   
  }

}
