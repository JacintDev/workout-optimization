import { Component, OnInit } from '@angular/core';
import { ScrollToService } from '../scroll-to.service';
import { Router } from '@angular/router';
import { AuthService } from '../AuthService';
import { map, Observable } from 'rxjs';
import { UserModel } from '../../models/UserModel';

@Component({
  selector: 'app-unathorized-nav',
  standalone: false,
  templateUrl: './unathorized-nav.component.html',
  styleUrl: './unathorized-nav.component.sass',
})
export class UnathorizedNavComponent implements OnInit {
  user$: Observable<UserModel | null>;
  constructor(
    private scrollToService: ScrollToService,
    private router: Router,
    private auth: AuthService
  ) {
    this.user$ = this.auth.currentUser$;
  }
  ngOnInit(): void {}
  scrollTo(fragment: string): void {
    this.router.navigate(['/welcome'], { fragment }).then(() => {
      this.scrollToService.scrollToElement(fragment);
    });
  }

  // isLoggedIn(): boolean {
  //   return this.auth.isLoggedIn().pipe(
  //     map((i) => {
  //       if (i) {
  //         return true;
  //       } else {
  //         return false;
  //       }
  //     })
  //   );
  // }
}
