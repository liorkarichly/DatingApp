import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
//Interceptor error
//the structure of what an 
//error intercept is or an HTTP interceptor is to be more accurate.

  constructor(private router: Router, private toasts: ToastrService) {}

  //Intercept the request that goes out or the 
  //response that comes back in the next where we handle the response.
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(

     //We want to catch error in switch case and return message or status code error
     //to calling HTTP request.
      catchError(error =>{
       
        if (error)
        {

          switch(error.status)//We eant to turn off the error and return something
          {

            case 400:        
              if (error.error.errors)    //CatchError//Object from JSON //Child in JSON
              {

                const modalStateErrors = [];
                
                for(const key in error.error.errors)//key - error object
                {

                  if (error.error.errors[key])
                  {
                    
                    modalStateErrors.push(error.error.errors[key]);
                  
                  }

                }
                
                //what we want to do is we're going to throw these model state errors back to the component.
                //And the reason that we're throwing these is because if we take our registration form, for example, then what we're going to want to do?
                throw modalStateErrors.flat();//flat - version from 2019: flatten array
                                
              }
              else if(typeof(error.error) === 'object')
              {

                this.toasts.error(error.statusText, error.status);
              
              }
              else
              {

                this.toasts.error(error.error, error.status);

              }

            break;
            
            case 401:

              this.toasts.error(error.statusText, error.status);
              break;

            case 404:

              this.router.navigateByUrl('/not-found');
              break;
            
            case 500:

            //Get the details of the error that we're going to get returned from the API.
            //NavigationExtras: a feature of the router where we can pass it, some states.
            const navegationExtra: NavigationExtras = {state: {error:error.error}};
            this.router.navigateByUrl('/server-error', navegationExtra);
            break;

            default:
              this.toasts.error('Something unexpected went worng');
              console.log(error);
              break;
            
          }

        }

        return throwError(error);

      }
      )
    )
  }
}
