import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
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

  constructor(private http:HttpClient) { }


  getMembers()
  {

    //Get return Observable<Object>
    return this.http.get<Member[]>(this.baseUrl + 'users');

  }

  getMember(username: string)
  {

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  
  }
  
}
