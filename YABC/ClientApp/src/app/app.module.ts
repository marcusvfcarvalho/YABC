import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CreateBlockComponent } from './create-block/create-block.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HomeComponent } from './home/home.component';
import { HttpClientModule } from '@angular/common/http';
import { MdbTooltipModule } from 'mdb-angular-ui-kit/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import {
  FontAwesomeModule,
  FaIconLibrary,
} from '@fortawesome/angular-fontawesome';

import {
  faEye as faEye,
} from '@fortawesome/free-solid-svg-icons';


@NgModule({
  declarations: [
    AppComponent,
    CreateBlockComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatPaginatorModule,
    MatTableModule,
    FormsModule,
    NgbModule,
    FontAwesomeModule,
    MdbTooltipModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { 
  constructor(library: FaIconLibrary) {
    library.addIcons(
      faEye
    );
  }

}
