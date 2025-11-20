import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ExerciseService {
  private link: string = 'http://localhost:5135/';
  constructor(private httpClient: HttpClient) {}

  getExerciseList(): Observable<any> {
    return this.httpClient.get(this.link + 'Exercise/');
  }
}
