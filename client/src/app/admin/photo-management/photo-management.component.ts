import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AdminService } from 'src/app/ClientServices/admin.service';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

photos: Photo[];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void
  {

    this.getPhotoForApproval();

  }


  getPhotoForApproval()
  {

    this.adminService.getPhotoForApproval().subscribe(

      photoss => {

        this.photos = photoss;

      }
    )

  }

  approvePhoto(photoId:number)
  {

    this.adminService.approvePhoto(photoId).subscribe(() =>
    {

      this.photos.splice(this.photos.findIndex(photoByIndex => photoByIndex.id === photoId), 1);

    });


  }

  rejectPhoto(photoId:number)
  {

    this.adminService.rejectPhoto(photoId).subscribe(() =>
    {

      this.photos.splice(this.photos.findIndex(photoByIndex => photoByIndex.id === photoId), 1);
      
    });

  }

}
