import { Component, OnDestroy, OnInit } from '@angular/core';
import { TrainingService } from '../services/training.service';
import { WorkoutSessionCount } from '../../models/WorkoutSessionCount';
import { Observable } from 'rxjs';
import { StartTrainingModel } from '../../models/StartTrainingModel';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-training',
  standalone: false,
  templateUrl: './training.component.html',
  styleUrls: ['./training.component.sass'],
})
export class TrainingComponent implements OnInit, OnDestroy {
  workoutSessions$!: Observable<WorkoutSessionCount[]>;
  private hubConnection!: signalR.HubConnection;
  public predictionMessage: string = '';
  isActiveTraining: boolean = false;
  trainingId: number = 0;
  training: StartTrainingModel = new StartTrainingModel();
  constructor(private trainingService: TrainingService) {}

  ngOnInit(): void {
    this.workoutSessions$ = this.trainingService.getWorkoutSessions();

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
    });
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
}
