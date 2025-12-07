import { Component, Inject, inject } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
import { FeedBack } from '../../models/FeedBack';
import { SummaryComponent } from '../summary/summary.component';

@Component({
  selector: 'app-feedback',
  standalone: false,
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.sass',
})
export class FeedbackComponent {
  selected: number = 0;
  results = [
    { name: 'Rettentően könnyű', value: 0 },
    { name: 'Könnyű', value: 1 },
    { name: 'Közepes', value: 2 },
    { name: 'Mérsékelten nehéz', value: 3 },
    { name: 'Nehéz', value: 4 },
  ];
  constructor(
    public dialogRef: MatDialogRef<FeedbackComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FeedBack,
    private dialog: MatDialog
  ) {}
  ShowSummary() {
    this.dialogRef.close();
    this.data.difficultyValue = this.results[this.selected].name;
    setTimeout(() => {
      this.dialog.open(SummaryComponent, {
        width: '600px',
        panelClass: 'custom-dialog',

        data: this.data,
      });
    }, 0);
  }
}
