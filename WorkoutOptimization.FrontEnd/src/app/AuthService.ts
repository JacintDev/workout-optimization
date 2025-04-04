import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  getToken(): string | null {
    return localStorage.getItem('token');
  }
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('expiration');
    localStorage.clear();
  }

  getUserRole(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    try {
      const tokenPayload = JSON.parse(atob(token.split('.')[1]));
      const roleKey =
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

      return tokenPayload[roleKey] || null;
    } catch (error) {
      console.error('Error parsing token payload:', error);
      return null;
    }
  }

  isLoggedIn(): Observable<boolean> {
    const token = this.getToken();
    if (!token) {
      return of(false);
    }

    return of(false); // TODO: Implement token expiration check

    //let expiration = localStorage.getItem('expiration');
    // if (!expiration) {
    //   return false;
    // }
    // const expirationTime = new Date(expiration).getTime();
    // const currentTime = Date.now(); // Convert to seconds
    // if (expirationTime > currentTime) {
    //   return true;
    // } else {
    //   return false;
    // }
  }
}
