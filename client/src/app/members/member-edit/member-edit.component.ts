import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/ClientServices/account.service';
import { MemberService } from 'src/app/ClientServices/member.service';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

@ViewChild('editForm') editForm: NgForm;//Specify the child selected that we're looking for
member: Member;//Getting the username from members
user: User;//I want to get the username spesific
@HostListener('window:beforeunload', ['$event']) unloadNotification($event: any)//Lisitner to operation on the page and Angular upload the element
{
 
  if(this.editForm.dirty)
  {

    $event.returnValue = true; 

  }

}

  constructor(private accountService: AccountService
      , private memberService:MemberService
      , private toastr: ToastrService)
       { 

        // I want to use in interceptor for to get the user from observable
        this.accountService.currentUser$.pipe(take(1))
                                    .subscribe(user => this.user = user);
       }                            //Take the user from service 

  ngOnInit(): void {
    this.loadMember();

  }

  loadMember()
  {
      //We get the user specific as a member
    this.memberService.getMember(this.user.username)
    .subscribe(member => this.member = member);

  }

  updateMember()
  {
      console.log("I inside");
    this.memberService.updateMemeber(this.member) 
      .subscribe(() =>
      {
        
         this.toastr.success('Profile updated successully');
         this.editForm.reset(this.member);

      });
   
  }

}
