import { Component } from '@angular/core';
import { AuthService } from './AuthService';
import { map, Observable } from 'rxjs';
import { UserModel } from '../models/UserModel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.sass',
})
export class AppComponent {
  title = 'WorkoutOptimization.FrontEnd';
  userLoggedIn$: Observable<boolean>;
  constructor(private auth: AuthService) {
    this.userLoggedIn$ = this.auth.currentUser$.pipe(
      map((user) => {
        return !!user;
      })
    );
  }
}
