import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/ClientServices/account.service';
import { MemberService } from 'src/app/ClientServices/member.service';
import { MessageService } from 'src/app/ClientServices/message.service';
import { PresenceService } from 'src/app/ClientServices/presence.service';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {

@ViewChild('memberTabs', {static: true})memberTabs:TabsetComponent;//but our message is, is a child component of our member detail page.
activeTab:TabDirective;
member:Member;
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];
messages: Message[] = [];
user:User;

//private memberService: MemberService - in constractor
  constructor(public presenceService: PresenceService
    , private route: ActivatedRoute
    ,private messageService: MessageService
    ,private accountService:AccountService
    ,private router: Router)
     { 

      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
     }
 

  ngOnInit(): void {

    //Input to route the details of member
    this.route.data.subscribe(data =>
      {

        this.member = data.member;

      });

    //this.loadMember();

    this.route.queryParams.subscribe(params =>
    {
      params.tab ? this.selectTab(params.tab): this.selectTab(0);

    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false//Click on image
      }
    ]
    this.galleryImages = this.getImages();

  }

  //Return array gallery of member
  getImages():NgxGalleryImage[]
  {

    const imageUrls =[];
    for(const photo of this.member.photos){
      imageUrls.push(
        {
          small:photo?.url,
          medium:photo?.url,
          big: photo?.url
        }
      )
    }

    return imageUrls;

  }

  //We Resolver THe Data Of Member - Put in app-routing.modle, member-detail.component, MemberDetailedResolver.resolver
// loadMember()
// {
  
//                                   //insert to route 'member/username'
//   this.memberService.getMember(this.route.snapshot.paramMap.get('username'))
//                               .subscribe(member => 
//                                 { this.member = member;
//                                   //this.galleryImages = this.getImages();
//                                 })

// }

onTabActivated(data: TabDirective)
{
  this.activeTab = data;//Have access to the information

  /**That length is equal to zero, because if they're switching between the tabs of a user
   *  and we already have messages loaded inside this component
   * , then obviously we're not going to dispose of them and then reload them again. */
  if(this.activeTab.heading === 'Messages' && this.messages.length === 0)
  {

    //this.loadMessages();
    this.messageService.createHubConnection(this.user, this.member.username);

  }
  else
  {

    this.messageService.stopConnection();
  }

}

loadMessages()
{

  this.messageService.getMessageThread(this.member.username).subscribe
  (response => {
    this.messages = response;
  });
  
}

selectTab(tabId: number)
{

  this.memberTabs.tabs[tabId].active = true;

}

ngOnDestroy(): void {
  this.messageService.stopConnection();

}

}
