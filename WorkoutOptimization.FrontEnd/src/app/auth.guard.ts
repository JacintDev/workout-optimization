import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { AuthService } from './AuthService';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']); // Ha nincs bejelentkezve, átirányítjuk a loginra
      return false;
    }
    const expectedRoles: string[] = route.data['roles']; // Több szerepkör is lehet
    const userRole = this.authService.getUserRole(); // A felhasználó aktuális szerepköre

    if (!userRole) {
      this.router.navigate(['/login']); // Ha nincs bejelentkezve, átirányítjuk a loginra
      return false;
    }

    // Ellenőrizzük, hogy a felhasználó szerepköre benne van-e az engedélyezettek között
    if (expectedRoles && !expectedRoles.includes(userRole)) {
      this.router.navigate(['/welcome']); // Ha nincs megfelelő jogosultság, átirányítjuk
      console.log('Jogosultság megtagadva!');

      return false;
    }
    console.warn('Jogosultság megadva: ' + userRole);

    return true; // Ha minden stimmel, akkor engedjük az útvonalra lépni
  }
}
