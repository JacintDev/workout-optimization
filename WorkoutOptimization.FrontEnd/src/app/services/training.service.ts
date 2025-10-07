import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WorkoutSessionCount } from '../../models/WorkoutSessionCount';
import { StartTrainingModel } from '../../models/StartTrainingModel';

@Injectable({
  providedIn: 'root',
})
export class TrainingService {
  private link: string = 'http://localhost:5135/';

  constructor(private http: HttpClient) {}

  getWorkoutSessions(): Observable<WorkoutSessionCount[]> {
    return this.http.get<WorkoutSessionCount[]>(
      `${this.link}Training/CountWorkoutSessions`
    );
  }
  startWebSocketSending(): Observable<any> {
    return this.http.get<any>(`${this.link}Websocket/Start/`);
  }
  stopWebSocketSending(): Observable<any> {
    return this.http.get<any>(`${this.link}Websocket/Stop/`);
  }
  startTraining(training: StartTrainingModel): Observable<any> {
    return this.http.post<any>(`${this.link}Training/CreateTraining`, training);
  }

  getActiveTraining(): Observable<any> {
    return this.http.get<any>(`${this.link}Training/GetActiveTraining`);
  }

  stopTraining(trainingId: number): Observable<any> {
    return this.http.put<any>(
      `${this.link}Training/StopTraining/${trainingId}`,
      null
    );
  }
}
