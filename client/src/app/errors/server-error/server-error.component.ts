import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

error:any;
  // we inside here is we need to inject our router 
  //into this so we've got access to the router state and we'll 
  //say router and router.
  constructor(private router: Router) 
  { 

    //We can access inside the constractor 
    const navigation = this.router.getCurrentNavigation();//Stop get navigation
    this.error = navigation?.extras?.state?.error;//Serial checking of navigation because whem the 
    //user is refresh so he lose information (error)
    
  }

  ngOnInit(): void {
  }

}
