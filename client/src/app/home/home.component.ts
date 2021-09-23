import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
registerMode = false;

  constructor() { }

  ngOnInit(): void {
  }

  registerToggle(){

    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: boolean)
  {

    //We sending the event in boolean because the function in cancel register, the emit is false 
    this.registerMode = event;
    //Passing data from child to parent

  }
}
