import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { StartTrainingModel } from '../../models/StartTrainingModel';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  private link: string = 'http://localhost:5135/';

  private currentUserTrainingCountSubject = new BehaviorSubject<number>(0);
  public currentUserTrainingCount$ =
    this.currentUserTrainingCountSubject.asObservable();

  constructor(private http: HttpClient) {}

  getUserTrainingCount(): Observable<number> {
    return this.http.get<number>(`${this.link}Training/CountTrainings`).pipe(
      tap((count) => {
        this.currentUserTrainingCountSubject.next(count);
      })
    );
  }

  startWebSocketSending(): Observable<any> {
    return this.http.get<any>(`${this.link}Websocket/Start/`);
  }

  startTraining(training: StartTrainingModel): Observable<any> {
    this.currentUserTrainingCountSubject.next(
      this.currentUserTrainingCountSubject.value + 1
    );
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

  stopWebSocketSending(): Observable<any> {
    return this.http.get<any>(`${this.link}Websocket/Stop/`);
  }
}
