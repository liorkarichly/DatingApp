import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusyService } from '../ClientServices/busy.service';
import { delay, finalize } from 'rxjs/operators';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService:BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //When we call to service and i want to telll to client that it busy
    this.busyService.busy();
    return next.handle(request).pipe(//Create fake delay

      delay(2000),
      finalize(() => {
        this.busyService.idle();
      })
    );
  }
}
