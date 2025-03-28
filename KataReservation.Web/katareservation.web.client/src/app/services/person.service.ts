import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Person } from '../models/person';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PersonService {

  constructor(private http: HttpClient) { }

  getPersons(): Observable<Person[]> {
    var response = this.http.get<Person[]>(environment.apiUrl);

        return this.ToPersonModel(response);
  }
  // Implementation of ToPersonModel function
 ToPersonModel(response: Observable<any[]>): Observable<Person[]> {
    return response.pipe(
      map(data => data.map(item => ({
        id: item.id || 0,
        firstName: this.(item.name || item.firstName || ''),
        lastName: this.extractLastName(item.name || item.lastName || '')
      })))
    );
  }
  private extractFirstName(fullName: string): string {
    return fullName.split(' ')[0] || '';
  }

  // Helper method to extract last name
  private extractLastName(fullName: string): string {
    const nameParts = fullName.split(' ');
    return nameParts.length > 1 ? nameParts.slice(1).join(' ') : '';
  }
  getPerson(id: number): Observable<Person> {
    return this.http.get<Person>(`${environment.apiUrl}/${id}`);
  }

  createPerson(person: Person): Observable<Person> {
    return this.http.post<Person>(environment.apiUrl, person);
  }

  updatePerson(person: Person): Observable<Person> {
    return this.http.put<Person>(`${environment.apiUrl}/${person.id}`, person);
  }

  deletePerson(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/${id}`);
  }
}
