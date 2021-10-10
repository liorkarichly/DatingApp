import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MemberService } from 'src/app/ClientServices/member.service';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
members$: Observable<Member[]>;

  constructor(private memberService: MemberService) { }

  ngOnInit(): void 
  {

    this.members$ = this.memberService.getMembers();
    //this.loadMembers();
  }

  // loadMembers()
  // {

  //   return this.memberService.getMembers()
  //   .subscribe(members => {this.members = members});//I use in sybscribe because the getMembers is return Observable<Member[]>

  // }
}
