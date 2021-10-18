import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, pipe } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { userParames } from '../_models/userParams';
import { AccountService } from './account.service';

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
    if(response)
    {

      return of(response);

    }
      let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
      
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

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
           .pipe(map(response => 
            {
              
              this.memberCache.set(Object.values(userParams).join('-'), response);
              return response
            
            }));

  }

  getMember(username: string)
  {

    // const member = this.members.find(currentUsername => currentUsername.username === username);

    // if(member !== undefined)
    // {

    //   return of(member);

    // }

    const member = [...this.memberCache.values()]
    .reduce((previousValueArray,currentValueElement) => previousValueArray.concat(currentValueElement.result), [])
    .find((member:Member) => member.username === username);

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

  private getPaginationHeaders(pageNumber:number, pageSize:number)
  {

    let params = new HttpParams();
    
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;

  }

  private getPaginatedResult<T>(url, params)
  {

    const paginatedResult:PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, {observe: 'response', params}).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if(response.headers.get('pagination') !== null)
        {

          paginatedResult.pagination = JSON.parse(response.headers.get('pagination'));
        }

        return paginatedResult;
      })
    );

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

    return  this.userParams;

  }
}
