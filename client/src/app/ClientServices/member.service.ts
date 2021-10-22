import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, pipe } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { userParames } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

//options,I use in const httpOption for Authorization of members because the our users is protected by authentication 
// const httpOptions = {
//   headers: new HttpHeaders({ 
//                    //Access|Allow             //Get token from user
//       Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class MemberService {
baseUrl = environment.apiUrl;
members: Member[] = [];
memberCache = new Map();
user:User;
userParams:userParames;

//Map -  When we want to store something with a key and value
      //, a good thing to use is a map.
      //And a map is like a dictionary, really.
  constructor(private http:HttpClient
    , private accountService:AccountService)
     {

      
      this.accountService.currentUser$.pipe(take(1)).subscribe(user  => 
        {
  
          this.user = user;
          this.userParams = new userParames(user);

        });

     }


  getMembers(userParams:userParames)
  {   

    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response)//If we have in our cache the results of that particular query.
    {

      return of(response);//We pass in a response.

    }
      let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
      
      params = params.append('minAge', userParams.minAge.toString());
      params = params.append('maxAge', userParams.maxAge.toString());
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
      
    // if(this.members.length > 0)//We want to return as an Observable because the member get for see him 
    // {

    //   return of(this.members);// as a map method
    // }
    //Get return Observable<Object>
    //return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      // map(members =>{
      //   this.members = members
      //   return this.members;
      // })
   // );

  //  return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(
  //     map(response => {
  //       this.paginatedResult.result = response.body;
  //       if(response.headers.get('pagination') !== null)
  //       {

  //         this.paginatedResult.pagination = JSON.parse(response.headers.get('pagination'));
  //       }

  //       return this.paginatedResult;
  //     })
  //   );

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
           .pipe(map(response => //Use in pipe() because that getPaginatedResult return Observable<PaginatedResult<Member[]>>
            {
              // we'll use the map function and we use this to transform the data that we get back.
              this.memberCache.set(Object.values(userParams).join('-'), response);//Input to cache, we'll have hold of the response inside here and then what we can do is say, let's stop, remember cash sets and we'll use the same key.
              return response
            
            }));
            /**
             * So the idea is that we go to our API, we go and get our members if we don't have them in our cash.
               But if we do have them in our cash, in the query is identical, then we just retrieve this from our cache.
             */

  }

  getMember(username: string)
  {

    // const member = this.members.find(currentUsername => currentUsername.username === username);

    // if(member !== undefined)
    // {

    //   return of(member);

    // }


    /**
     * const member = ['...'this.memberCache.values()]
     * The spread operator (in form of ellipsis) can be used in two ways:
       Initializing arrays and objects from another array or object
       Object de-structuring
       The spread operator is most widely used for method arguments in form of rest parameters where more than 1 value is expected
     * 
     * 
     * 
     */
    const member = [...this.memberCache.values()]                     //Initialize of array is empty (in first time) and we concat the element (members) that appearing for the first time.
    .reduce((previousValueArray,currentValueElement) => previousValueArray.concat(currentValueElement.result), [])//Reduce our array into something else.We don't want an array of arrays with opportunity objects and other things inside it.
    .find((member:Member) => member.username === username);//Without our user, checking if we have a member in array

      if(member)
      {

        return of(member);

      }
      
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  
  }
  
  updateMemeber(member:Member){

    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => 
      {
        const index = this.members.indexOf(member);//Find the index of member
        this.members[index] = member;//Update member
      })
    )
     

  }

  setMainPhoto(photoId: number)
  {

      //We 'put' something to user so we need to get back something, and we dont get back anything
      return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
    
  }

  deletePhoto(photoId:number)
  {

    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
    
  }

  getUserPararms()
  {

      return this.userParams;
  }

  setUserParams(params:userParames)
  {

    this.userParams = params;

  }

  resetUserParams()
  {

    this.userParams = new userParames(this.user);

    return  this.userParams;//Return new user params 

  }

  addLike(username: string)
  {

    return this.http.post(this.baseUrl + 'likes/' + username, {});

  }

  getLikes(predicate: string, pageNumber, pageSize)
  {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    //return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate=' +  predicate);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);
  }
}
