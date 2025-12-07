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

  summaryTranslation: Record<number, string> = {
    0: 'Javasolt a könnyebb súly választása!',
    1: 'Javasolt a nehezebb súly választása!',
    2: 'A Pulzusod alapján javasolt a több pihenő tartása, vagy kisebb súly választása!',
    3: 'Javasolt a jelenlegi edzésprogram folytatása.',
    4: 'Javasolt a nehezebb súly, vagy több ismétlés választása!',
    99: 'Hiba történt az értékelés során.',
  };

  private difficultyTranslation: Record<string, number> = {
    'Rettentően könnyű': 0,
    Könnyű: 1,
    Közepes: 2,
    'Mérsékelten nehéz': 3,
    Nehéz: 4,
  };

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

  Calculate(): number {
    const sum = this.data.correctExercise + this.data.incorrectExercise;
    if (sum === 0) return 99;
    if (this.data.pulseMessage == 'A pulzusod magas!') return 2;
    if (sum < 6) return 0;
    if (sum > 10) return 1;
    if (
      sum >= 6 &&
      sum <= 8 &&
      this.difficultyTranslation[this.data.difficultyValue!] > 3
    )
      return 3;
    if (
      sum >= 6 &&
      sum <= 8 &&
      this.difficultyTranslation[this.data.difficultyValue!] < 4
    )
      return 4;
    if (
      sum >= 9 &&
      sum <= 10 &&
      this.difficultyTranslation[this.data.difficultyValue!] < 4
    )
      return 4;
    if (
      sum >= 9 &&
      sum <= 10 &&
      this.difficultyTranslation[this.data.difficultyValue!] > 3
    )
      return 4;
    return 99;
  }
}
