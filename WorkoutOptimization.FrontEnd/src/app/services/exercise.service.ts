import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment.prod';

@Injectable({
  providedIn: 'root',
})
export class ExerciseService {
  private link: string = environment.apiUrl;
  constructor(private httpClient: HttpClient) {}

  getExerciseList(): Observable<any> {
    return this.httpClient.get(this.link + 'Exercise/');
  }
}
