import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StartTrainingModel } from '../../models/StartTrainingModel';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  private link: string = 'http://localhost:5135/';

  constructor(private http: HttpClient) {}

  startWebSocketSending(): Observable<any> {
    return this.http.get<any>(`${this.link}Websocket/Start/`);
  }

  startTraining(training: StartTrainingModel): Observable<any> {
    return this.http.post<any>(`${this.link}Training/CreateTraining`, training);
  }

  getActiveTraining(): Observable<any> {
    return this.http.get<any>(`${this.link}Training/GetActiveTraining`);
  }

  stopTraining(trainingId: number): Observable<any> {
    return this.http.post<any>(`${this.link}Training/StopTraining`, trainingId);
  }

  stopWebSocketSending(): Observable<any> {
    return this.http.get<any>(`${this.link}Websocket/Stop/`);
  }
}
