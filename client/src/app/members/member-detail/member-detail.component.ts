import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { MemberService } from 'src/app/ClientServices/member.service';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
member:Member;
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];

  constructor(private memberService: MemberService, private route: ActivatedRoute) { }

  ngOnInit(): void {

    this.loadMember();
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

loadMember()
{
  
                                  //insert to route 'member/username'
  this.memberService.getMember(this.route.snapshot.paramMap.get('username'))
                              .subscribe(member => 
                                { this.member = member;
                                  this.galleryImages = this.getImages();
                                })

}

}