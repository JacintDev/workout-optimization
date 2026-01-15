import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { UserModel } from '../models/UserModel';
import { UserUpdateModel } from '../models/UserUpdateModel';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserModel | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  getToken(): string | null {
    return localStorage.getItem('token');
  }
  logout(): Observable<void> {
    return new Observable((observer) => {
      localStorage.removeItem('token');
      localStorage.removeItem('expiration');
      localStorage.clear();

      this.currentUserSubject.next(null);

      observer.next();
      observer.complete();
    });
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

  private setUser(user: any): UserModel {
    let u = user as UserModel;
    return u;
  }

  isLoggedIn(): Observable<boolean> {
    const token = this.getToken();
    if (!token) {
      return of(false);
    }

    // const headers = new HttpHeaders({
    //   'Content-Type': 'application/json',
    //   Authorization: `Bearer ${token}`,
    // });
    return this.http.get<any>(`${environment.apiUrl}Auth/IsLoggedIn`).pipe(
      map((resp) => {
        if (resp.isLoggedIn === true) {
          this.currentUserSubject.next(this.setUser(resp.user));
          return true;
        }
        this.currentUserSubject.next(null);

        return false;
      }),
      catchError((error) => {
        return of(false);
      })
    );
  }
  userUpdate(userUpdateModel: UserUpdateModel): Observable<any> {
    return this.http.put<any>(
      `${environment.apiUrl}Auth/UpdateUser`,
      userUpdateModel
    );
  }
}
