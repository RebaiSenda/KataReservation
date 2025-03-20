import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Person, CreatePersonRequest, UpdatePersonRequest } from '../models/person.model';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private apiUrl = '/api/persons';

  constructor(private http: HttpClient) { }

  getPersons(): Observable<Person[]> {
    return this.http.get<Person[]>(this.apiUrl);
  }

  getPerson(id: number): Observable<Person> {
    return this.http.get<Person>(`${this.apiUrl}/${id}`);
  }

  createPerson(request: CreatePersonRequest): Observable<Person> {
    return this.http.post<Person>(this.apiUrl, request);
  }

  updatePerson(id: number, request: UpdatePersonRequest): Observable<Person> {
    return this.http.put<Person>(`${this.apiUrl}/${id}`, request);
  }

  deletePerson(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
