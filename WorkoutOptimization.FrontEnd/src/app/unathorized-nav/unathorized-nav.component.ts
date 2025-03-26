import { Component } from '@angular/core';
import { ScrollToService } from '../scroll-to.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unathorized-nav',
  standalone: false,
  templateUrl: './unathorized-nav.component.html',
  styleUrl: './unathorized-nav.component.sass',
})
export class UnathorizedNavComponent {
  constructor(
    private scrollToService: ScrollToService,
    private router: Router
  ) {}
  scrollTo(fragment: string): void {
    this.router.navigate(['/welcome'], { fragment }).then(() => {
      this.scrollToService.scrollToElement(fragment);
    });
  }
}
