import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { LogoutDialogComponent } from '../logout-dialog/logout-dialog.component';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.sass',
})
export class HeaderComponent {
  readonly dialog = inject(MatDialog);
  openDialog(): void {
    this.dialog.open(LogoutDialogComponent, {
      width: '1000px',
      panelClass: 'custom-dialog',
      data: { name: 'Kilépés', question: 'Biztosan ki akarsz lépni?' },
    });
  }
}
