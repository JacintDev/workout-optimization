import { Component, OnDestroy, OnInit } from '@angular/core';
import { StartTrainingModel } from '../../models/StartTrainingModel';
import { TrainingService } from '../services/training.service';
import * as signalR from '@microsoft/signalr';
import { PulsemeasureService } from '../services/pulsemeasure.service';
import { map, Observable } from 'rxjs';
import { PulseViewModel } from '../../models/PulseViewModel';
import { MatDialog } from '@angular/material/dialog';
import { FeedbackComponent } from '../feedback/feedback.component';
import { FeedBack } from '../../models/FeedBack';
import { log } from 'three/src/nodes/TSL.js';

@Component({
  selector: 'app-training-new-toggle',
  standalone: false,
  templateUrl: './training-new-toggle.component.html',
  styleUrls: ['./training-new-toggle.component.sass'],
})
export class TrainingNewToggleComponent implements OnInit, OnDestroy {
  private hubConnection!: signalR.HubConnection;
  public predictionMessage: string = '';
  isActiveTraining: boolean = false;
  trainingId: number = 0;
  training: StartTrainingModel = new StartTrainingModel();
  predictionCount: number = 0;
  correctExercise: number = 0;
  incorrectExercise: number = 0;
  pulse$!: Observable<PulseViewModel>;
  lastPulseObj: PulseViewModel = new PulseViewModel();
  showPulse = false;
  constructor(
    private trainingService: TrainingService,
    private pulseService: PulsemeasureService,
    private dialog: MatDialog
  ) {
    this.pulseLastSave();
  }

  ngOnInit(): void {
    //SignalR
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5135/exercisehub')
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
      })
      .catch((err) => console.error('SignalR error:', err));

    this.hubConnection.on('ReceivePrediction', (message: string) => {
      this.predictionMessage = message;
      this.predictionCount++;
      if (this.predictionMessage == 'Helyes') {
        this.correctExercise++;
      } else {
        this.incorrectExercise++;
      }
    });
    this.pulse$ = this.pulseService.pulse$;
  }
  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => console.log('SignalR Disconnected'));
    }
  }

  startTraining() {
    this.startWebSocketSending();
    this.training.start = new Date().toISOString();
    this.training.isActive = true;
    this.training.exerciseId = 1;

    this.trainingService.startTraining(this.training).subscribe({
      next: (res) => this.getActiveTraining(),
      error: (err) => console.log(err),
    });
  }
  private startWebSocketSending() {
    this.trainingService.startWebSocketSending().subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }
  private getActiveTraining() {
    this.trainingService.getActiveTraining().subscribe({
      next: (res) => {
        this.isActiveTraining = true;
        this.trainingId = res.trainingId;
        console.log(res);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  stopTraining() {
    this.stopWebSocketSending();
    this.trainingService.stopTraining(this.trainingId).subscribe({
      next: (res) => {
        this.isActiveTraining = false;
        this.predictionCount = 0;
        this.predictionMessage = '';
        console.log(res);
      },
      error: (err) => console.log(err),
    });
  }

  private stopWebSocketSending() {
    this.trainingService.stopWebSocketSending().subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  startPulseMeasurement() {
    this.pulseService.startWebSocketPulseMeasurement().subscribe({
      next: (res) => {
        this.showPulse = true;
        setTimeout(() => this.stopPulseMeasurement(), 10000);
        setTimeout(() => (this.showPulse = false), 12000);
        console.log(res);
      },
      error: (err) => console.log(err),
    });
  }
  private stopPulseMeasurement() {
    this.pulseService.stopWebSocketPulseMeasurement().subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  private pulseLastSave() {
    this.pulseService.pulse$.subscribe({
      next: (res) => {
        this.lastPulseObj = res;
        console.log(this.lastPulseObj.pulse);
      },
    });
  }

  feedback(): void {
    let data: FeedBack = new FeedBack();
    data.correctExercise = this.correctExercise;
    data.incorrectExercise = this.incorrectExercise;
    data.averagePulse = this.lastPulseObj.pulse;
    data.pulseMessage = this.lastPulseObj.message;
    data.selectionValue = 'unselected';

    this.dialog.open(FeedbackComponent, {
      width: '400px',
      panelClass: 'custom-dialog',
      data,
    });
  }
}
