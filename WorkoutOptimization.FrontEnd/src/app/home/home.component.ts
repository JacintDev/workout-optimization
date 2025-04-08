import { Component } from '@angular/core';
import { AuthService } from '../AuthService';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.sass',
})
export class HomeComponent {
  constructor(private auth: AuthService) {
    this.auth.currentUser$.subscribe((user) => {
      console.log(user);
    });
  }
}
