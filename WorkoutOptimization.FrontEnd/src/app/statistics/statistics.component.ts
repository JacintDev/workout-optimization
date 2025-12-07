import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Chart } from 'chart.js';
import { DailyWeight } from '../../models/DailyWeight';
import { HomeService } from '../services/home.service';
import { combineLatest, map, Observable } from 'rxjs';
import { TrainingService } from '../services/training.service';
import { ExerciseResultReturnedValueModel } from '../../models/ExerciseResultReturnedValueModel';
import { WorkoutSessionCount } from '../../models/WorkoutSessionCount';
import { CombinedDataRow } from '../../models/CombinedDataRow';

@Component({
  selector: 'app-statistics',
  standalone: false,
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.sass'],
})
export class StatisticsComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('weightChart') weightChart!: ElementRef<HTMLCanvasElement>;
  @ViewChild('accuracyChart') accuracyChart!: ElementRef<HTMLCanvasElement>;

  viewReady = false;

  private weightChartInstance?: Chart;
  private accuracyChartInstance?: Chart;

  currentUserMonthlyWeights: Array<DailyWeight> = [];

  $userActiveDaysCount!: Observable<any>;
  currentUserTrainingCount$!: Observable<number>;
  currentUserCorrectRepetitionsCount$!: Observable<any>;
  currentUserIncorrectRepetitionsCount$!: Observable<any>;

  dailyTrainingResults$!: Observable<ExerciseResultReturnedValueModel[]>;
  dailyTrainingResults: ExerciseResultReturnedValueModel[] = [];

  combinedData$!: Observable<CombinedDataRow[]>;
  workoutSessions$!: Observable<WorkoutSessionCount[]>;

  constructor(
    private homeService: HomeService,
    private trainingService: TrainingService
  ) {}

  ngOnInit(): void {
    // ----- SÚLY GRAFIKON ADATOK -----
    this.homeService.getUserMonthlyWeights().subscribe((data) => {
      data.map((item: any) => {
        const dw = new DailyWeight();
        dw.date = item.date.split('T')[0];
        dw.weight = item.weight;
        this.currentUserMonthlyWeights.push(dw);
      });
      this.tryCreateWeightChart();
    });

    // ----- SZÁMLÁLÓS STATOK -----
    this.$userActiveDaysCount = this.homeService.getUserActiveDaysCount();
    this.currentUserTrainingCount$ = this.homeService.currentUserTrainingCount$;
    this.currentUserCorrectRepetitionsCount$ =
      this.homeService.getUserCorrectRepetitionsCount();
    this.currentUserIncorrectRepetitionsCount$ =
      this.homeService.getUserIncorrectRepetitionsCount();

    // ----- EDZÉSSZÁM + NAPI EREDMÉNYEK -----
    this.workoutSessions$ = this.trainingService.getWorkoutSessions();
    this.dailyTrainingResults$ = this.trainingService.getDailyTrainingResults();

    // Napi eredmények elmentése + grafikon hívás
    this.dailyTrainingResults$.subscribe((data) => {
      console.log('dailyTrainingResults$ adat:', data);
      console.log('length:', data?.length);
      this.dailyTrainingResults = data ?? [];
      this.tryCreateAccuracyChart();
    });

    // Ha kell még táblázathoz:
    this.combinedData$ = combineLatest([
      this.workoutSessions$,
      this.dailyTrainingResults$,
    ]).pipe(
      map(
        ([sessions, results]: [
          WorkoutSessionCount[],
          ExerciseResultReturnedValueModel[]
        ]) => {
          const allDates = Array.from(
            new Set([
              ...sessions.map((s) => s.date),
              ...results.map((r) => r.date),
            ])
          );

          return allDates.map((date) => {
            const session = sessions.find((s) => s.date === date);
            const result = results.find((r) => r.date === date);

            return {
              date,
              count: session?.count ?? null,
              correct: result?.correct ?? null,
              inCorrect: result?.inCorrect ?? null,
            } as CombinedDataRow;
          });
        }
      )
    );
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.tryCreateWeightChart();
    this.tryCreateAccuracyChart();
  }

  // ---------- SÚLY GRAFIKON ----------

  private tryCreateWeightChart(): void {
    if (!this.viewReady) return;
    if (!this.weightChart) return;
    if (!this.currentUserMonthlyWeights.length) return;
    this.createWeightChart();
  }

  private createWeightChart(): void {
    const ctx = this.weightChart.nativeElement.getContext('2d');
    if (!ctx || this.currentUserMonthlyWeights.length === 0) return;

    const firstDate = new Date(this.currentUserMonthlyWeights[0].date);
    const year = firstDate.getFullYear();
    const monthIndex = firstDate.getMonth(); // 0..11

    const daysInMonth = new Date(year, monthIndex + 1, 0).getDate();

    const labels = Array.from({ length: daysInMonth }, (_, i) =>
      (i + 1).toString()
    );

    const dailyWeights: (number | null)[] = new Array(daysInMonth).fill(null);

    this.currentUserMonthlyWeights.forEach((w) => {
      const d = new Date(w.date);
      const day = d.getDate(); // 1..31
      if (day >= 1 && day <= daysInMonth) {
        dailyWeights[day - 1] = w.weight;
      }
    });

    if (this.weightChartInstance) {
      this.weightChartInstance.destroy();
    }

    this.weightChartInstance = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [
          {
            label: 'Testsúly (kg)',
            data: dailyWeights,
            borderColor: 'rgb(255, 99, 132)',
            backgroundColor: 'rgba(255, 99, 132, 0.1)',
            tension: 0.4,
            fill: true,
            borderWidth: 3,
            pointRadius: 5,
            pointBackgroundColor: 'rgb(255, 99, 132)',
            pointBorderColor: '#fff',
            pointBorderWidth: 2,
            pointHoverRadius: 7,
            spanGaps: true,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: true,
            position: 'top',
          },
          title: {
            display: true,
            text: 'Súlyváltozás (havi)',
            font: {
              size: 16,
            },
          },
          tooltip: {
            callbacks: {
              label: function (context) {
                return context.dataset.label + ': ' + context.parsed.y + ' kg';
              },
            },
          },
        },
        scales: {
          y: {
            beginAtZero: false,
            min: this.currentUserMonthlyWeights[0].weight - 10,
            max: this.currentUserMonthlyWeights[0].weight + 10,
            ticks: {
              callback: function (value: any) {
                return value + ' kg';
              },
            },
          },
        },
      },
    });
  }

  // ---------- PONTOSSÁG GRAFIKON (correct vs incorrect) ----------

  private tryCreateAccuracyChart(): void {
    console.log('tryCreateAccuracyChart', {
      viewReady: this.viewReady,
      hasCanvas: !!this.accuracyChart,
      len: this.dailyTrainingResults.length,
    });

    if (!this.viewReady) return;
    if (!this.accuracyChart) return;

    // Itt már mindig megpróbáljuk kirajzolni (ha nincs adat, 0-s oszlopok)
    this.createAccuracyChart();
  }

  private createAccuracyChart(): void {
    const ctx = this.accuracyChart.nativeElement.getContext('2d');
    if (!ctx) return;

    let labels: string[] = [];
    let correctData: number[] = [];
    let incorrectData: number[] = [];

    if (this.dailyTrainingResults.length === 0) {
      // NINCS adat: az elmúlt 30 nap, mindenhol 0
      const endDate = new Date();
      const startDate = new Date();
      startDate.setDate(endDate.getDate() - 29);

      for (let i = 0; i < 30; i++) {
        const d = new Date(startDate);
        d.setDate(startDate.getDate() + i);

        const day = d.getDate();
        const month = d.getMonth() + 1;

        labels.push(`${month}.${day}.`);
        correctData.push(0);
        incorrectData.push(0);
      }
    } else {
      // VAN adat: eredeti logikád, kicsit biztonságosabban
      const parsed = this.dailyTrainingResults.map((item) => {
        const d = this.parseHungarianDate(item.date); // pl. "2025. 11. 04."
        return { ...item, dateObj: d };
      });

      // legkésőbbi dátum
      const maxDate = parsed.reduce(
        (max, item) => (item.dateObj > max ? item.dateObj : max),
        parsed[0].dateObj
      );

      const endDate = new Date(maxDate);
      const startDate = new Date(maxDate);
      startDate.setDate(endDate.getDate() - 29); // elmúlt 30 nap

      const mapByDate: Record<string, ExerciseResultReturnedValueModel> = {};
      parsed.forEach((item) => {
        const key = item.dateObj.toISOString().split('T')[0];
        mapByDate[key] = {
          date: item.date,
          correct: item.correct,
          inCorrect: item.inCorrect,
        };
      });

      for (let i = 0; i < 30; i++) {
        const d = new Date(startDate);
        d.setDate(startDate.getDate() + i);

        const key = d.toISOString().split('T')[0];
        const day = d.getDate();
        const month = d.getMonth() + 1;

        labels.push(`${month}.${day}.`);

        const item = mapByDate[key];
        if (item) {
          correctData.push(item.correct);
          incorrectData.push(item.inCorrect);
        } else {
          correctData.push(0);
          incorrectData.push(0);
        }
      }
    }

    if (this.accuracyChartInstance) {
      this.accuracyChartInstance.destroy();
    }

    this.accuracyChartInstance = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: 'Helyes végrehajtás',
            data: correctData,
            backgroundColor: 'rgba(76, 175, 80, 0.8)', // zöld
            stack: 'stack1',
          },
          {
            label: 'Helytelen végrehajtás',
            data: incorrectData,
            backgroundColor: 'rgba(244, 67, 54, 0.8)', // piros
            stack: 'stack1',
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top',
            labels: {
              color: '#fff',
            },
          },
          title: {
            display: true,
            text: 'Gyakorlat pontosság (elmúlt 30 nap)',
            color: '#fff',
          },
          tooltip: {
            callbacks: {
              footer: (items: any[]) => {
                const total = items.reduce(
                  (sum, item) => sum + item.parsed.y,
                  0
                );
                return `Összesen: ${total}`;
              },
            },
          },
        },
        scales: {
          x: {
            stacked: true,
            ticks: {
              color: '#fff',
            },
          },
          y: {
            stacked: true,
            beginAtZero: true,
            ticks: {
              color: '#fff',
            },
          },
        },
      },
    });
  }

  private parseHungarianDate(input: string): Date {
    // "2025. 11. 04." -> Date
    const parts = input
      .split('.')
      .map((p) => p.trim())
      .filter((p) => p);
    const year = Number(parts[0]);
    const month = Number(parts[1]) - 1;
    const day = Number(parts[2]);
    return new Date(year, month, day);
  }

  // ---------- Destroy ----------

  ngOnDestroy(): void {
    if (this.weightChartInstance) {
      this.weightChartInstance.destroy();
    }
    if (this.accuracyChartInstance) {
      this.accuracyChartInstance.destroy();
    }
  }
}
