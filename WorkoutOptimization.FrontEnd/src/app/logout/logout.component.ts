import { Component } from '@angular/core';
import { AuthService } from '../AuthService';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-logout',
  standalone: false,
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.sass',
})
export class LogoutComponent {
  snack: MatSnackBar;
  constructor(
    private auth: AuthService,
    private router: Router,
    snack: MatSnackBar
  ) {
    this.snack = snack;

    this.auth.logout().subscribe(() => {
      this.snack
        .open('Sikeres kijelentkezés!', 'OK', { duration: 2000 })
        .afterDismissed()
        .subscribe(() => {
          this.router.navigate(['/welcome']);
        });
    });
  }
}
