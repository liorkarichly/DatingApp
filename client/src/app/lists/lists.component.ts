import { Component, OnInit } from '@angular/core';
import { MemberService } from '../ClientServices/member.service';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
members:Partial<Member[]>;//Create members optional in future
predicate = 'liked';//Default and we'll set this to liked.
pageNumber = 1;
pageSize = 10;
pagination:Pagination;

  constructor(private memberService: MemberService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes()
  {

  this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(
    response =>
    {

      this.members = response.result;
      this.pagination = response.pagination;

    }
  );
 
 }

 pageChanged(event:any)
 {
   
  this.pageNumber = event.page;
  this.loadLikes();
  
 }


}
