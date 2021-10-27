import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/ClientServices/account.service';
import { MemberService } from 'src/app/ClientServices/member.service';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member:Member;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user:  User;

  constructor(private accountService:AccountService
    , private memberService: MemberService) 
  { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {

    this.initializeUploader();

  }

  //File the upload has successfully uploaded
  initializeUploader(){
   
    this.uploader = new FileUploader(
    {
      url:this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5:true,
      allowedFileType:['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10*1024*1024 //10 MB

    });

    this.uploader.onAfterAddingFile = (file)=> {//WE dont confirm the upload of parameter as file because we need to djustment to our API cause configuration and allow credentials to go up with our request
      file.withCredentials = false;             //and we dont need it because we use in bearer token for this
    
    }

    //If the upload is success
    this.uploader.onSuccessItem = (item, response, status, header) =>
    {

      if(response)
      {

        const photo:Photo = JSON.parse(response);

        this.member.photos.push(photo);

        if(photo.isMain)
        {

          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
          
        }
      }

    }

  }

  fileOverBase(event: any)
  {

    this.hasBaseDropZoneOver = event;//Set our drop zone inside the template

  }

  setMainPhoto(photo: Photo)
  {

    this.memberService.setMainPhoto(photo.id).subscribe(() => 
      {

        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        this.member.photoUrl = photo.url;
        this.member.photos.forEach(photoIsMain => 
          {

            if(photoIsMain.isMain)
            {

              photoIsMain.isMain = false;

            }

            if(photoIsMain.id == photo.id)
            {

              photoIsMain.isMain = true;

            }

          })

      });
    
  }

  deletePhoto(photoId: number)
  {

    this.memberService.deletePhoto(photoId).subscribe(() =>
    {
      //Filter of photos and return match ID photo
      this.member.photos = this.member.photos.filter(photoDeleted => photoDeleted.id != photoId);
    
    });
    
  }

}
