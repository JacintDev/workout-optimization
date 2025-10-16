import { Component, OnDestroy, OnInit } from '@angular/core';
import { TrainingService } from '../services/training.service';
import { WorkoutSessionCount } from '../../models/WorkoutSessionCount';
import { combineLatest, map, Observable } from 'rxjs';
import { StartTrainingModel } from '../../models/StartTrainingModel';
import * as signalR from '@microsoft/signalr';
import { ExerciseResultReturnedValueModel } from '../../models/ExerciseResultReturnedValueModel';
import { CombinedDataRow } from '../../models/CombinedDataRow';

@Component({
  selector: 'app-training',
  standalone: false,
  templateUrl: './training.component.html',
  styleUrls: ['./training.component.sass'],
})
export class TrainingComponent implements OnInit {
  workoutSessions$!: Observable<WorkoutSessionCount[]>;
  dailyTrainingResults$!: Observable<ExerciseResultReturnedValueModel[]>;
  combinedData$!: Observable<CombinedDataRow[]>;
  // private hubConnection!: signalR.HubConnection;
  // public predictionMessage: string = '';
  // isActiveTraining: boolean = false;
  // trainingId: number = 0;
  // training: StartTrainingModel = new StartTrainingModel();
  constructor(private trainingService: TrainingService) {}

  ngOnInit(): void {
    this.workoutSessions$ = this.trainingService.getWorkoutSessions();
    this.dailyTrainingResults$ = this.trainingService.getDailyTrainingResults();

    this.combinedData$ = combineLatest([
      this.workoutSessions$,
      this.dailyTrainingResults$,
    ]).pipe(
      map(
        ([sessions, results]: [
          WorkoutSessionCount[],
          ExerciseResultReturnedValueModel[]
        ]) => {
          // összes dátum kigyűjtése, duplikátumok nélkül
          const allDates = Array.from(
            new Set([
              ...sessions.map((s) => s.date),
              ...results.map((r) => r.date),
            ])
          );

          // új, összefűzött lista létrehozása
          return allDates.map((date) => {
            const session = sessions.find((s) => s.date === date);
            const result = results.find((r) => r.date === date);

            return {
              date,
              count: session?.count ?? null,
              correct: result?.correct ?? null,
              inCorrect: result?.inCorrect ?? null,
            };
          });
        }
      )
    );

    // //SignalR
    // this.hubConnection = new signalR.HubConnectionBuilder()
    //   .withUrl('http://localhost:5135/exercisehub')
    //   .build();
    // this.hubConnection
    //   .start()
    //   .then(() => {
    //     console.log('SignalR connection started');
    //   })
    //   .catch((err) => console.error('SignalR error:', err));

    // this.hubConnection.on('ReceivePrediction', (message: string) => {
    //   this.predictionMessage += message;
    // });
  }

  calculateAccuracy(correct: number, inCorrect: number): number {
    let total = correct + inCorrect;
    if (total === 0) return 0;
    return Math.round((correct / total) * 100);
  }

  badgeColor(correct: number, inCorrect: number): string {
    if (correct + inCorrect === 0) return 'badge-warning';
    if (correct >= inCorrect) {
      return 'badge-success';
    } else {
      return 'badge-red';
    }
  }
  // ngOnDestroy(): void {
  //   if (this.hubConnection) {
  //     this.hubConnection.stop().then(() => console.log('SignalR Disconnected'));
  //   }
  // }

  // startTraining() {
  //   this.startWebSocketSending();
  //   this.training.start = new Date().toISOString();
  //   this.training.isActive = true;
  //   this.training.exerciseId = 1;

  //   this.trainingService.startTraining(this.training).subscribe({
  //     next: (res) => this.getActiveTraining(),
  //     error: (err) => console.log(err),
  //   });
  // }
  // private startWebSocketSending() {
  //   this.trainingService.startWebSocketSending().subscribe({
  //     next: (res) => console.log(res),
  //     error: (err) => console.log(err),
  //   });
  // }
  // private getActiveTraining() {
  //   this.trainingService.getActiveTraining().subscribe({
  //     next: (res) => {
  //       this.isActiveTraining = true;
  //       this.trainingId = res.trainingId;
  //       console.log(res);
  //     },
  //     error: (err) => {
  //       console.log(err);
  //     },
  //   });
  // }

  // stopTraining() {
  //   this.stopWebSocketSending();
  //   this.trainingService.stopTraining(this.trainingId).subscribe({
  //     next: (res) => {
  //       this.isActiveTraining = false;
  //       console.log(res);
  //     },
  //     error: (err) => console.log(err),
  //   });
  // }

  // private stopWebSocketSending() {
  //   this.trainingService.stopWebSocketSending().subscribe({
  //     next: (res) => console.log(res),
  //     error: (err) => console.log(err),
  //   });
  // }
}
