import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { PersonListComponent } from './components/persons/person-list/person-list.component';
import { PersonFormComponent } from './components/persons/person-form/person-form.component';
import { RoomListComponent } from './components/rooms/room-list/room-list.component';
import { RoomFormComponent } from './components/rooms/room-form/room-form.component';
import { BookingFormComponent } from './components/bookings/booking-form/booking-form.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'persons', component: PersonListComponent, canActivate: [AuthGuard] },
  { path: 'persons/new', component: PersonFormComponent, canActivate: [AuthGuard] },
  { path: 'persons/edit/:id', component: PersonFormComponent, canActivate: [AuthGuard] },
  { path: 'rooms', component: RoomListComponent, canActivate: [AuthGuard] },
  { path: 'rooms/new', component: RoomFormComponent, canActivate: [AuthGuard] },
  { path: 'rooms/edit/:id', component: RoomFormComponent, canActivate: [AuthGuard] },
  { path: 'bookings/new', component: BookingFormComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
