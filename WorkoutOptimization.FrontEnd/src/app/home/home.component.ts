import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AuthService } from '../AuthService';
import {
  FormBuilder,
  Validators,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserUpdateModel } from '../../models/UserUpdateModel';
import { UserModel } from '../../models/UserModel';
import { Chart } from 'chart.js/auto';
import { StartTrainingModel } from '../../models/StartTrainingModel';
import * as signalR from '@microsoft/signalr';
import { map, Observable } from 'rxjs';
import { HomeService } from '../services/home.service';
import { DailyWeight } from '../../models/DailyWeight';
import { PulsemeasureService } from '../services/pulsemeasure.service';
import { PulseViewModel } from '../../models/PulseViewModel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.sass'],
})
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('weightChart') weightChart!: ElementRef<HTMLCanvasElement>;
  viewReady = false;

  private weightChartInstance?: Chart;
  showPulse = false;
  pulse$!: Observable<PulseViewModel>;
  profileNeedSetup$!: Observable<boolean>;
  profilePropertiesNeedSetup = true;
  isSettedUpDailyWeight: boolean = false;
  private hubConnection!: signalR.HubConnection;
  public predictionMessage: string = '';
  userUpdate = new UserUpdateModel();
  user$!: Observable<UserModel | null>;
  currentUserTrainingCount$!: Observable<number>;
  isActiveTraining: boolean = false;
  trainingId: number = 0;
  training: StartTrainingModel = new StartTrainingModel();
  currentUserActiveDaysCount$!: Observable<any>;
  currentUserMonthlyWeights: Array<DailyWeight> = [];
  getUserLastTrainingDate$!: Observable<string>;

  fitnessLevel: any = [
    { value: 1, viewValue: 'Kezdő' },
    { value: 2, viewValue: 'Középhaladó' },
    { value: 3, viewValue: 'Haladó' },
  ];

  private _formBuilder = inject(FormBuilder);

  today: string = new Date().toISOString().split('T')[0];

  constructor(
    private auth: AuthService,
    private http: HttpClient,
    private homeService: HomeService,
    private pulseService: PulsemeasureService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.profileNeedSetup$ = this.auth.currentUser$.pipe(
      map((user) => !user || !user.height || !user.weight || !user.restPulse)
    );
    this.pulse$ = this.pulseService.pulse$;
    this.user$ = this.auth.currentUser$;
    this.homeService.getUserTrainingCount().subscribe();
    this.currentUserTrainingCount$ = this.homeService.currentUserTrainingCount$;
    this.getUserLastTrainingDate$ = this.homeService.getUserLastTrainingDate();
    this.currentUserActiveDaysCount$ =
      this.homeService.getUserActiveDaysCount();
    this.homeService.isSettedUpDailyWeight().subscribe((data) => {
      this.isSettedUpDailyWeight = data;
    });

    this.homeService.getUserMonthlyWeights().subscribe((data) => {
      data.map((item: any) => {
        const dw = new DailyWeight();
        dw.date = item.date.split('T')[0];
        dw.weight = item.weight;
        this.currentUserMonthlyWeights.push(dw);
      });
      this.tryCreateChart();
    });
    this.pulseLastSave();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
  }

  createWeightChart(): void {
    const ctx = this.weightChart.nativeElement.getContext('2d');
    if (!ctx || this.currentUserMonthlyWeights.length === 0) return;

    // hónap + év az első rekordból
    const firstDate = new Date(this.currentUserMonthlyWeights[0].date);
    const year = firstDate.getFullYear();
    const monthIndex = firstDate.getMonth(); // 0..11

    // hány napos ez a hónap?
    const daysInMonth = new Date(year, monthIndex + 1, 0).getDate();

    // X tengely: "1", "2", ..., "31"
    const labels = Array.from({ length: daysInMonth }, (_, i) =>
      (i + 1).toString()
    );

    // alap: minden napra nincs adat
    const dailyWeights: (number | null)[] = new Array(daysInMonth).fill(null);

    // ahol van adat, oda beírjuk a súlyt
    this.currentUserMonthlyWeights.forEach((w) => {
      const d = new Date(w.date);
      const day = d.getDate(); // 1..31
      if (day >= 1 && day <= daysInMonth) {
        dailyWeights[day - 1] = w.weight;
      }
    });

    // ha már van chart, töröljük
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
              label: function (context: any) {
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

  private tryCreateChart(): void {
    console.log('tryCreateChart hívva');
    console.log('viewReady:', this.viewReady);
    console.log('weightChart elem:', this.weightChart);
    console.log('súlyok száma:', this.currentUserMonthlyWeights.length);
    console.log('súlyok:', this.currentUserMonthlyWeights);

    if (!this.viewReady) {
      console.log('View még nem ready');
      return;
    }
    if (!this.weightChart) {
      console.log('weightChart elem nem található');
      return;
    }
    if (!this.currentUserMonthlyWeights.length) {
      console.log('Nincs súly adat');
      return;
    }

    console.log('Chart létrehozása...');
    this.createWeightChart();
  }
  ngOnDestroy(): void {
    if (this.weightChartInstance) {
      this.weightChartInstance.destroy();
    }
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => console.log('SignalR Disconnected'));
    }
  }

  // === Validator függvények ===
  minDateValidator(minDate: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;

      const inputDate = new Date(value);
      return inputDate >= minDate ? null : { minDate: true };
    };
  }

  maxDateValidator(maxDate: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;

      const inputDate = new Date(value);
      return inputDate <= maxDate ? null : { maxDate: true };
    };
  }

  // === Formok ===
  firstFormGroup = this._formBuilder.group({
    firstCtrl: [
      '',
      [
        Validators.required,
        this.minDateValidator(new Date('1920-01-01')),
        this.maxDateValidator(new Date()),
      ],
    ],
  });

  secondFormGroup = this._formBuilder.group({
    secondCtrl: [
      '',
      [Validators.required, Validators.min(30), Validators.max(200)],
    ],
  });

  thirdFormGroup = this._formBuilder.group({
    thirdCtrl: [
      '',
      [Validators.required, Validators.min(100), Validators.max(220)],
    ],
  });

  fourthFormGroup = this._formBuilder.group({
    fourthCtrl: ['', Validators.required],
  });

  isLinear = true;

  onSubmit(): void {
    if (
      this.firstFormGroup.valid &&
      this.secondFormGroup.valid &&
      this.thirdFormGroup.valid &&
      this.fourthFormGroup.valid
    ) {
      this.auth.userUpdate(this.userUpdate).subscribe({
        next: (res) => {
          const currentUrl = this.router.url;
          this.router
            .navigateByUrl('/', { skipLocationChange: true })
            .then(() => {
              this.router.navigateByUrl(currentUrl);
            });
        },
        error: (err) => console.log(err),
      });
    }
  }

  startPulseMeasurement() {
    this.pulseService.startWebSocketPulseMeasurement().subscribe({
      next: (res) => {
        this.showPulse = true;
        this.pulseLastSave();

        setTimeout(() => this.stopPulseMeasurement(), 10000);
        setTimeout(() => (this.showPulse = false), 10000);
        console.log(res);
      },
      error: (err) => console.log(err),
    });
  }
  private stopPulseMeasurement() {
    this.pulseService.stopWebSocketPulseMeasurement().subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  private pulseLastSave() {
    console.log('ELINDUL A METÓDUS!');

    this.pulseService.pulse$.subscribe({
      next: (res) => {
        this.userUpdate.restPulse = res.pulse;
        console.log(res.pulse);
      },
    });
  }

  onSubmitDailyWeight() {
    this.homeService.submitDailyWeight(this.userUpdate.weight!).subscribe({
      next: (res) => {
        this.isSettedUpDailyWeight = true;
      },
      error: (err) => console.log(err),
    });
  }
}
