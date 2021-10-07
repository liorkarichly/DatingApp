import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../ClientServices/account.service';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

  //We need the interceptor for pass token for to be options of member.

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

//We get the our user (accountService) for to know when the user is connnect and use with her service, in log in
  constructor(private accountService: AccountService) {}

  //Use in token of user, i need to get him for use
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User;

    //We want to out the subscribe of current user and get what's inside the observeable
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => currentUser = user);//.subscribe() -  we dont need anumore because when we take one of observable so we can unsubscribe
    //take - get one from this observable and then we can unsubscribe
    //.pipe(take(1)) - we want to complete after we've received one of these current users.
    if(currentUser)
    {
      //if we have curren user so we want to set authorization bwcaue the our user need auth and request can clone it
      request = request.clone(
        {
          setHeaders:{
            Authorization: 'Bearer ' + currentUser.token
          }
        }
      )
    }
    return next.handle(request);
  }
}
