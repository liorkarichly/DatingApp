import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {

  @Input() label: string;
  @Input() maxDate: Date;

  //Partial<BsDatepickerConfig> - Every single property inside this type is going to be optional.
  bsConfig: Partial<BsDatepickerConfig>;
  
  constructor(@Self() public ngControl: NgControl) 
  {

    this.ngControl.valueAccessor = this;
    this.bsConfig ={
      containerClass:'theme-red',
      dateInputFormat: 'DD MMMM YYYY'//MMMM - Full name of month, MM - Number of month
    }

  }

  writeValue(obj: any): void {
    
  }
  registerOnChange(fn: any): void {
   
  }
  registerOnTouched(fn: any): void {
    
  }
  

}
