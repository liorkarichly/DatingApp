import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/ClientServices/account.service';
import { MemberService } from 'src/app/ClientServices/member.service';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { userParames } from 'src/app/_models/userParams';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
//members$: Observable<Member[]>;
members: Member[];
pagination:Pagination;
userParams: userParames;
user:User;
genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];


  constructor(private memberService: MemberService)
   { 

    //We pass to member service
    // this.accountService.currentUser$.pipe(take(1)).subscribe(user  => 
    //   {

    //     this.user = user;
    //     this.userParams = new userParames(user);
    //   });

    this.userParams = this.memberService.getUserPararms();// I dont want to lost the my filter when i insert to profile of member and when ill back to list' ill stay with the same filter 
   }

  ngOnInit(): void 
  {

    //this.members$ = this.memberService.getMembers();
    //this.loadMembers();
   this.loadMembers();

  }

  // loadMembers()
  // {

  //   return this.memberService.getMembers()
  //   .subscribe(members => {this.members = members});//I use in sybscribe because the getMembers is return Observable<Member[]>

  // }

  //Call to members per page
  loadMembers()
  {

    this.memberService.setUserParams(this.userParams);
    this.memberService.getMembers(this.userParams)
    .subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  //Next page
  pageChanged(event: any)
  {

    this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadMembers();
    
  }

  //Initialize agian of list members
  resetFilters()
  {

      this.userParams  = this.memberService.resetUserParams();
      this.loadMembers();
      
  }

 
}
