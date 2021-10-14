import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})

//ControlValueAccessor - Interface, between API to Angular
//formControlName - Native in DOM, we need access in our componnet
export class TextInputComponent implements ControlValueAccessor {

  @Input() label: string;
  @Input() type = 'text';

  //@Self() - I want to inject to our componnet for 'first' text, Angular will do when it's looking at dependency injection is going to look inside the hierarchy of things that it can inject.
  //f there's an injector that matches this to this already got inside its dependency injection container.
  //We want our text input components to be self-contained and we don't want Angular to try and fetch us another instance of what we're doing here.
  //We always want this to be self-contained.
  constructor(@Self() public ngControl: NgControl) 
  { 

    this.ngControl.valueAccessor = this;
    
  }
  writeValue(obj: any): void {

  }
  registerOnChange(fn: any): void {

  }
  registerOnTouched(fn: any): void {
 
  }
 


}
