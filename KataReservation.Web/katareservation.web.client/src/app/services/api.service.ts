// src/app/services/api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room,CreateRoomRequest  } from '../models/room';
import { Booking ,CreateBookingRequest} from '../models/booking';
import { Person ,CreatePersonRequest} from '../models/person';
@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'https://localhost:5002/api';

  constructor(private http: HttpClient) { }

  // Room services
    getRooms(): Observable<Room[]> {
      return this.http.get<Room[]>(`${this.apiUrl}/rooms`);
    }

  createRoom(room: CreateRoomRequest): Observable<Room> {
    return this.http.post<Room>(`${this.apiUrl}/rooms`, room);
  }

  // Booking services
  getBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/bookings`);
  }

  createBooking(booking: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(`${this.apiUrl}/bookings`, booking);
  }

  deleteBooking(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/bookings/${id}`);
  }

  // Person services
  getPersons(): Observable<Person[]> {
    return this.http.get<Person[]>(`${this.apiUrl}/persons`);
  }

  createPerson(person: CreatePersonRequest): Observable<Person> {
    return this.http.post<Person>(`${this.apiUrl}/persons`, person);
  }
}
