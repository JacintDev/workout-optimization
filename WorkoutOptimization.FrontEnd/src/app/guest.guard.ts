import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './AuthService';
import { map, Observable } from 'rxjs';
import { UserModel } from '../models/UserModel';

@Injectable({
  providedIn: 'root',
})
export class GuestGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> {
    return this.authService.isLoggedIn().pipe(
      map((isLoggedIn) => {
        console.log(isLoggedIn);

        if (isLoggedIn) {
          this.router.navigate(['/home']); // Ha be van jelentkezve, átirányítjuk a welcome oldalra
          return false; // Ha be van jelentkezve, akkor nem engedjük az útvonalra lépni
        }
        return true;
      })
    );
  }
}
