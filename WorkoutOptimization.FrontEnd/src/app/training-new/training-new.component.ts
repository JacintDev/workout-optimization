import { Component } from '@angular/core';

@Component({
  selector: 'app-training-new',
  standalone: false,
  templateUrl: './training-new.component.html',
  styleUrl: './training-new.component.sass',
})
export class TrainingNewComponent {
  exercises = [
    { name: 'Kalapács bicepsz', value: 1 },
    { name: 'Bicepsz állva rúddal', value: 1 },
    { name: 'Bicepsz állva egykezes súlyzóval', value: 1 },
    { name: 'Bicepsz ülve egykezes súlyzóval', value: 1 },
    { name: 'Bicepsz koncentrált egykezes súlyzóval', value: 1 },
    { name: 'Tricepsz letolás csigán', value: 1 },
    { name: 'Tricepsz nyújtás egykezes súlyzóval fej fölött', value: 1 },
    { name: 'Tricepsz fekvőtámasz', value: 1 },
    { name: 'Tricepsz tolódzkodás', value: 1 },
    { name: 'Fekvenyomás', value: 1 },
    { name: 'Tárogatás', value: 1 },
    { name: 'Mellről nyomás', value: 1 },
    { name: 'Tárogatás ferde padon', value: 1 },
    { name: 'Tárogatás negatív padon', value: 1 },
    { name: 'Evezés döntött törzzsel', value: 1 },
  ];
  selected: number | null = null;
}
