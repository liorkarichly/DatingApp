import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { User } from '../_models/user';

/**
 * 1. Services are injectable
 * 2. Services are singelton
 */
@Injectable({
  providedIn: 'root'//Metadata, Singelton
})
export class AccountService {
  
  //The service does't get destroyed until our application is closed. 
baseUrl = 'https://localhost:5001/api/'; //Base URL Property, Make Request To Our API
//baseUrl:TYPE
//baseUrl= SET 
private currentUserSource = new ReplaySubject<User>(1);//kind of like a buffer object is going to store the values inside,ReplaySubject<User>(1) <Type>(Amount - Size in buffer) 
currentUser$ = this.currentUserSource.asObservable(); //$ sign the object is observable


//Inject the HTTP Client into our account service
  //Get from angular 
  constructor(private http:HttpClient) { }

  login(model:any)
  {

      //Subscribe with pipe and map
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User)=> {
        const user = response;//Get user from response
        if (user)//Check if we have user or not
        {
          // Populate the object(user) in local storage of the browser
          localStorage.setItem('user', JSON.stringify(user));
                              //key     //take object that back
          this.currentUserSource.next(user);//Current user, how to set the next value 
        }

      })

    );

  }

  setCurrentUser(user:User)
  {

    this.currentUserSource.next(user);

  }

  logout()
  {

    //Remove from browser the user by key
    localStorage.removeItem('user');
    this.currentUserSource.next(null);

  }

}

/**
 * Components are diffrent when we move from component to component and angular.
 * They are destroyed as soon as theyre not in use, whereas a service is a singelton and we typically
 * user services for making our HTTP request, but well find many uses for them as we develop this application.
 */
