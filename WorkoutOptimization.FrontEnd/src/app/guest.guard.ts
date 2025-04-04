import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './AuthService';

@Injectable({
  providedIn: 'root',
})
export class GuestGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']); // Ha már be van jelentkezve, akkor átnavigáljuk a főoldalra
      return false;
    }

    return true; // Ha nincs bejelentkezve, akkor beléphet
  }
}
