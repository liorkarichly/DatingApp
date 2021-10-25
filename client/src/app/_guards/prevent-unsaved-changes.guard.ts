import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { ConfirmService } from '../ClientServices/confirm.service';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  
  /**
   *
   */
  constructor(private confirmService: ConfirmService) {
   
    
  }
  
  canDeactivate(
    component: MemberEditComponent): Observable<boolean> | boolean {
    if(component.editForm.dirty)
    {

      return this.confirmService.confirm();
  
    }
      return true;//when i eant to return without nothing because i want that the item pass 

  }
  
}
