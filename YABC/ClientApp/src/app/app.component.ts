import { Component, OnInit } from '@angular/core';
import { Block } from './models/block';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public lastBlock!: Block;

  constructor() { }

  ngOnInit() {
   
  }


}