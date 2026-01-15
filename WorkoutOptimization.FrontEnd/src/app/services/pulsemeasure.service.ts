import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable, NgZone } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { PulseViewModel } from '../../models/PulseViewModel';
import { environment } from '../../environment/environment.prod';

@Injectable({
  providedIn: 'root',
})
export class PulsemeasureService {
  private link: string = environment.apiUrl;
  private hub!: HubConnection;
  private pulseSub = new Subject<PulseViewModel>();
  pulse$ = this.pulseSub.asObservable();

  constructor(private http: HttpClient, private zone: NgZone) {
    this.hub = new HubConnectionBuilder()
      .withUrl('http://localhost:5135/exercisehub')
      .withAutomaticReconnect()
      .build();

    this.hub.on('ReceivePulse', (pulse: PulseViewModel) => {
      this.zone.run(() => this.pulseSub.next(pulse));
      console.log(pulse);
    });

    this.start();
  }

  startWebSocketPulseMeasurement(): Observable<any> {
    return this.http.get<any>(`${this.link}/Websocket/startPulseDataSend`);
  }

  stopWebSocketPulseMeasurement(): Observable<any> {
    return this.http.get<any>(`${this.link}/Websocket/stopPulseDataSend`);
  }

  private start() {
    this.hub.start().catch(() => {
      setTimeout(() => this.start(), 2000);
    });
  }
}
