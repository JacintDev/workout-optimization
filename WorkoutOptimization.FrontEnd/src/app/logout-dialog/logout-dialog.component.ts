import { Component, Inject, inject, ViewEncapsulation } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout-dialog',
  standalone: false,
  templateUrl: './logout-dialog.component.html',
  styleUrl: './logout-dialog.component.sass',
  encapsulation: ViewEncapsulation.None,
})
export class LogoutDialogComponent {
  /**
   *
   */
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private router: Router
  ) {}
  logout() {
    this.router.navigate(['/logout']);
  }
}
