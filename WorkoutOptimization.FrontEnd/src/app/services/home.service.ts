import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
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
  getUserActiveDaysCount(): Observable<any> {
    return this.http
      .get<any>(`${this.link}Training/CountWorkoutSessions`)
      .pipe(map((arr) => arr.length));
  }

  submitDailyWeight(weight: number): Observable<any> {
    return this.http.post<any>(`${this.link}DailyWeight/Post`, {
      weight: weight,
    });
  }

  getUserLastTrainingDate(): Observable<string> {
    return this.http.get<string>(`${this.link}Training/GetLastTrainingDate`, {
      responseType: 'text' as 'json',
    });
  }

  getUserMonthlyWeights(): Observable<any> {
    return this.http.get<any>(`${this.link}DailyWeight/GetUserMonthlyWeights`);
  }

  getUserCorrectRepetitionsCount(): Observable<any> {
    return this.http.get<any>(
      `${this.link}ExerciseResult/GetUserCorrectRepetitions`
    );
  }

  getUserIncorrectRepetitionsCount(): Observable<any> {
    return this.http.get<any>(
      `${this.link}ExerciseResult/GetUserIncorrectRepetitions`
    );
  }

  isSettedUpDailyWeight(): Observable<any> {
    return this.http.get<any>(`${this.link}DailyWeight/IsSettedUpDailyWeight`);
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
