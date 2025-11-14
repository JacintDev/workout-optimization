import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FeedBack } from '../../models/FeedBack';

@Component({
  selector: 'app-summary',
  standalone: false,
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.sass',
})
export class SummaryComponent {
  proposal: string = '';
  spinnerEnabled: boolean = true;
  displayLoadingText: string = 'Gyakorlat elemzése...';
  displayResult: boolean = false;
  constructor(
    public dialogRef: MatDialogRef<SummaryComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FeedBack
  ) {
    setTimeout(() => {
      this.displayLoadingText = 'Gyakorlat kiértékelése...';
      this.disableSpinner();
    }, 2000);
  }

  private disableSpinner(): void {
    setTimeout(() => {
      this.displayLoadingText = '';
      this.spinnerEnabled = false;
      this.displayResult = true;
    }, 2000);
  }

  close() {
    this.dialogRef.close();
  }

  private Calculate(): number {
    const sum = this.data.correctExercise + this.data.incorrectExercise;
    if (sum < 6) {
      return 0;
    }
    if (sum > 12) {
      return 1;
    }
    return 0;
  }
}
