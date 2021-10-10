import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
busyRequestCount = 0;//Count the multiple request, i want to increment this counter and decrement
  constructor(private spinnerServices: NgxSpinnerService) { }

  busy()
  {

    this.busyRequestCount++;

    //undefined - name to spinner -> i dont to set name to each spinner
    this.spinnerServices.show(undefined, 
      {
          type:'ball-spin-fade-rotating',
          bdColor: 'rgba(255,255,255,0)',
          color:'#333333'
      });
  }

  idle()
  {

      this.busyRequestCount--;

      if(this.busyRequestCount <= 0)
      {

        this.busyRequestCount = 0;
        this.spinnerServices.hide();

      }
  }

}
