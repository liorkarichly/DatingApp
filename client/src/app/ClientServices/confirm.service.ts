import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
 
  bsModelRef: BsModalRef;

  constructor(private modalService: BsModalService) { }
  
  confirm(title = 'Confirmation'
  , message = 'Are you sure want to do this?'
  , buttonOkText =' Ok'
  , buttonCancelText = 'Cancel'):Observable<boolean>
  {

    const config = {
      initialState: 
      {

        title,
        message,
        buttonOkText,
        buttonCancelText

      }

    };

    this.bsModelRef = this.modalService.show(ConfirmDialogComponent, config);//Show dialog

    return new Observable<boolean>(this.getResult());

  }


  private getResult()
  {

    return (observer) => {

      const subscription = this.bsModelRef.onHidden.subscribe(() => 
      {

        observer.next(this.bsModelRef.content.result);
        observer.complete();
        
      });

      return {
        unsubscribe()
        {

          subscription.unsubscribe();

        }
      }
     
    }

  }

}
