import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

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

  constructor(private http:HttpClient) { }


  getMembers()
  {

    if(this.members.length > 0)//We want to return as an Observable because the member get for see him 
    {

      return of(this.members);// as a map method
    }
    //Get return Observable<Object>
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members =>{
        this.members = members
        return this.members;
      })
    );

  }

  getMember(username: string)
  {

    const member = this.members.find(currentUsername => currentUsername.username === username);

    if(member !== undefined)
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
}
