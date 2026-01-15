import { Component, OnDestroy, OnInit } from '@angular/core';
import { StartTrainingModel } from '../../models/StartTrainingModel';
import { TrainingService } from '../services/training.service';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environment/environment';
import { PulsemeasureService } from '../services/pulsemeasure.service';
import { map, Observable, Subscription, timer } from 'rxjs';
import { PulseViewModel } from '../../models/PulseViewModel';
import { MatDialog } from '@angular/material/dialog';
import { FeedbackComponent } from '../feedback/feedback.component';
import { FeedBack } from '../../models/FeedBack';
import { log } from 'three/src/nodes/TSL.js';
import { ActivatedRoute } from '@angular/router';
import { ExerciseService } from '../services/exercise.service';

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
  exerciseId: number = 0;
  showPulse = false;
  sub!: Subscription;
  showPulseButton: boolean = false;
  clicked: boolean = false;
  exerciseTranslations: Record<string, string> = {
    BicepsCurl: 'Kalapács bicepsz',
    ShoulderPress: 'Vállból nyomás',
    HammerCurl: 'Kalapács bicepsz',
  };

  constructor(
    private trainingService: TrainingService,
    private pulseService: PulsemeasureService,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private exerciseService: ExerciseService
  ) {
    this.pulseLastSave();
    this.exerciseId = Number(this.route.snapshot.paramMap.get('id'));
    console.log(this.exerciseId);
  }

  ngOnInit(): void {
    //SignalR
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}exercisehub`)
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
    this.training.exerciseId = this.exerciseId;
    this.training.axis = 2;

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
      next: (res) => {
        this.HidePulseButton();
      },
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
      next: (res) => this.feedback(),
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
    this.exerciseService.getExerciseList().subscribe((x: any[]) => {
      const found = x
        .find((l: any) => l.exerciseId == this.exerciseId)
        ?.name.toString();
      data.selectionValue =
        this.exerciseTranslations[found] || 'Ismeretlen gyakorlat';
      this.dialog.open(FeedbackComponent, {
        width: '400px',
        panelClass: 'custom-dialog',
        data,
      });
    });
  }
  userClicked(): void {
    this.clicked = true;
  }
  HidePulseButton(): void {
    this.showPulseButton = true;
    this.sub = timer(5000).subscribe(() => {
      if (this.clicked) {
        this.showPulseButton = false;
        this.clicked = false;
        return;
      }
      this.showPulseButton = false;
      this.feedback();
    });
  }
}
