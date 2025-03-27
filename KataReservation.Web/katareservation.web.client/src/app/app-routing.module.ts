import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomListComponent } from './rooms/room-list/room-list.component';
import { PersonListComponent } from './persons/person-list/person-list.component';
import { BookingListComponent } from './bookings/booking-list/booking-list.component';

const routes: Routes = [
  { path: '', redirectTo: '/bookings', pathMatch: 'full' },
  { path: 'rooms', component: RoomListComponent },
  { path: 'persons', component: PersonListComponent },
  { path: 'bookings', component: BookingListComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }