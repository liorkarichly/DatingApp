import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

/**
 * 1. Services are injectable
 * 2. Services are singelton
 */
@Injectable({
  providedIn: 'root'//Metadata, Singelton
})
export class AccountService {
  
  //The service does't get destroyed until our application is closed. 
//baseUrl = 'https://localhost:5001/api/'; //Base URL Property, Make Request To Our API
//baseUrl:TYPE
//baseUrl= SET 
baseUrl = environment.apiUrl;
private currentUserSource = new ReplaySubject<User>(1);//kind of like a buffer object is going to store the values inside,ReplaySubject<User>(1) <Type>(Amount - Size in buffer) 
currentUser$ = this.currentUserSource.asObservable(); //$ sign the object is observable


//Inject the HTTP Client into our account service
  //Get from angular 
  constructor(private http:HttpClient,
    private presenceService: PresenceService) { }

  login(model:any)
  {

      //Subscribe with pipe and map
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User)=> {
        const user = response;//Get user from response
        if (user)//Check if we have user or not
        {
          // Populate the object(user) in local storage of the browser
          // localStorage.setItem('user', JSON.stringify(user));
          //                     //key     //take object that back
          // this.currentUserSource.next(user);//Current user, how to set the next value 

          this.setCurrentUser(user);
          this.presenceService.createHubConnection(user);
        }

        //return user; for to print in consol 
      })

    );

  }

  setCurrentUser(user:User)
  {

    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;//Take the roles
    Array.isArray(roles)? user.roles = roles:user.roles.push(roles);
    
    localStorage.setItem('user', JSON.stringify(user));

    this.currentUserSource.next(user);

  }

  logout()
  {

    //Remove from browser the user by key
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();

  }

  register(model:any)
  {

    return this.http.post(this.baseUrl + 'account/register' , model).pipe(
      map((user: User) => {//We casting because, the map get object and it didnt know that he get User object, any for get around for any problem 

          if(user)
          {

           this.setCurrentUser(user);
           this.presenceService.createHubConnection(user);

          }
        })
    )
  }

  /*We close the of attempts, 
  what we need is to take a look inside our 
  token from our account service.
  And we decoded the token for identity*/
  getDecodedToken(token)
  {

    /*atob - This is just going to allow us to decode the information inside what 
             the token is returned.
             As the token is not encrypted, the signature is the only part is encrypte
    */

      return JSON.parse(atob(token.split('.')[1]))//We take the part of payload
      /**
       * Token with 3 parts
       * 1.title
       * 2.payload
       * 3.signature
       */

  }

}

/**
 * Components are diffrent when we move from component to component and angular.
 * They are destroyed as soon as theyre not in use, whereas a service is a singelton and we typically
 * user services for making our HTTP request, but well find many uses for them as we develop this application.
 */
