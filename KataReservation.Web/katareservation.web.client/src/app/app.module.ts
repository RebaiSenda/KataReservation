import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { PersonListComponent } from './components/persons/person-list/person-list.component';
import { PersonFormComponent } from './components/persons/person-form/person-form.component';
import { RoomListComponent } from './components/rooms/room-list/room-list.component';
import { RoomFormComponent } from './components/rooms/room-form/room-form.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    PersonListComponent,
    PersonFormComponent,
    RoomListComponent,
    RoomFormComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
