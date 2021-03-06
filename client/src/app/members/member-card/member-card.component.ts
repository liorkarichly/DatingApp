import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MemberService } from 'src/app/ClientServices/member.service';
import { PresenceService } from 'src/app/ClientServices/presence.service';
import { Member } from 'src/app/_models/member';


//encapsulation - Template of CSS
//have 3 types:
//1.ViewEncapsulation.Emulated - use in CSS like a native beaviour - default
//2.ViewEncapsulation.None - use in CSS global withput encapsulation
//3.iewEncapsulation.ShadowDom - use in 'Shaow DOM v1' for encapsulation types
//i use in default encapsulation
@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
@Input() member: Member;//Child -> get data from Parent
  
constructor(private memberService: MemberService
          , private toastr: ToastrService
          , public presenceService: PresenceService) { }

  ngOnInit(): void {
  }

  addLike(member: Member)
  {

    this.memberService.addLike(member.username).subscribe(() => 
    this.toastr.success('You have liked ' + member.knownAs)
    );
  }

}
