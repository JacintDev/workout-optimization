import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-count-up',
  standalone: false,
  templateUrl: './count-up.component.html',
  styleUrl: './count-up.component.sass',
})
export class CountUpComponent {
  private _end: number = 0;
  value = 0;

  @Input() duration: number = 3000;

  @Input() set end(value: number | null) {
    // ha null jön (betöltéskor), ne csináljunk semmit
    if (value == null) {
      return;
    }

    this._end = value;
    this.startAnimation();
  }

  private startAnimation() {
    const start = 0;
    const end = this._end;
    const duration = this.duration;
    const startTime = performance.now();

    const step = (currentTime: number) => {
      const elapsed = currentTime - startTime;
      const progress = Math.min(elapsed / duration, 1);

      this.value = Math.floor(start + (end - start) * progress);

      if (progress < 1) {
        requestAnimationFrame(step);
      }
    };

    requestAnimationFrame(step);
  }
}
