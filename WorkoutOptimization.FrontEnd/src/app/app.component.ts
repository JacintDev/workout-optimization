import { Component } from '@angular/core';
import { AuthService } from './AuthService';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.sass',
})
export class AppComponent {
  title = 'WorkoutOptimization.FrontEnd';
  logged: boolean = false;
  constructor(private auth: AuthService) {
    this.auth.currentUser$.subscribe((user) => {
      if (user?.UserId != null) {
        this.logged = true;
      } else {
        this.logged = false;
      }
    });
  }
}
