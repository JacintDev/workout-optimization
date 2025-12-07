import { Component } from '@angular/core';

@Component({
  selector: 'app-authorized-navbar',
  standalone: false,
  templateUrl: './authorized-navbar.component.html',
  styleUrl: './authorized-navbar.component.sass',
})
export class AuthorizedNavbarComponent {
  isMenuOpen = false;
  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }
}
