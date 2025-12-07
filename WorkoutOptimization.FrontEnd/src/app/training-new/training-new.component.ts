import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ExerciseService } from '../services/exercise.service';

@Component({
  selector: 'app-training-new',
  standalone: false,
  templateUrl: './training-new.component.html',
  styleUrl: './training-new.component.sass',
})
export class TrainingNewComponent {
  exercises = [{ name: '', value: 0 }];
  selected: number | null = null;
  exerciseTranslations: Record<string, string> = {
    BicepsCurl: 'Kalapács bicepsz',
    ShoulderPress: 'Vállból nyomás',
    HammerCurl: 'Kalapács bicepsz',
  };
  constructor(
    private router: Router,
    private exerciseService: ExerciseService
  ) {
    exerciseService.getExerciseList().subscribe({
      next: (data) => {
        this.exercises = data.map((x: any) => {
          return {
            name: this.exerciseTranslations[x.name] || x.name,
            value: x.exerciseId,
          };
        });
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
  goToCreateTrainingNewToggle() {
    if (this.selected) {
      this.router.navigate(['/createtrainingnewtoggle', this.selected]);
    }
  }
}
