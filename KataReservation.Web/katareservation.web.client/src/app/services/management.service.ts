import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user'; 
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';  

@Injectable({ providedIn: 'root' })
export class ManagementService {
    private apiUrl = `${environment.apiUrl}/bff/user`; 

    constructor(private http: HttpClient) {}

    async getUser(): Promise<User> {
        try {
            const response = await firstValueFrom(this.http.get<User>(this.apiUrl)); 
            if (!response) {
                throw new Error('User data is undefined');
            }
            return {
                name: response.name,
                logoutUrl: response.logoutUrl,
            };
        } catch (error) {
            window.location.replace(`${environment.apiUrl}/bff/login?returnUrl=${window.location.pathname}`);
            throw error; 
        }
    }
}