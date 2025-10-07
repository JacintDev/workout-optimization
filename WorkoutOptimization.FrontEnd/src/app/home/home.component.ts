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

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.sass'],
})
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('weightChart') weightChart!: ElementRef<HTMLCanvasElement>;

  private weightChartInstance?: Chart;
  profileNeedSetup$!: Observable<boolean>;
  profilePropertiesNeedSetup = true;
  private hubConnection!: signalR.HubConnection;
  public predictionMessage: string = '';
  userUpdate = new UserUpdateModel();
  user$!: Observable<UserModel | null>;
  isActiveTraining: boolean = false;
  trainingId: number = 0;
  training: StartTrainingModel = new StartTrainingModel();

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
    private homeService: HomeService
  ) {}

  ngOnInit(): void {
    this.profileNeedSetup$ = this.auth.currentUser$.pipe(
      map((user) => !user || !user.height || !user.weight)
    );
    this.user$ = this.auth.currentUser$;

    //SignalR
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5135/exercisehub')
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
      })
      .catch((err) => console.error('SignalR error:', err));

    this.hubConnection.on('ReceivePrediction', (message: string) => {
      this.predictionMessage = message;
    });
  }

  ngAfterViewInit(): void {
    this.createWeightChart();
  }

  createWeightChart(): void {
    const ctx = this.weightChart.nativeElement.getContext('2d');
    if (ctx) {
      this.weightChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
          labels: [
            'Jan',
            'Feb',
            'Már',
            'Ápr',
            'Máj',
            'Jún',
            'Júl',
            'Aug',
            'Szep',
            'Okt',
            'Nov',
            'Dec',
          ],
          datasets: [
            {
              label: 'Testsúly (kg)',
              data: [85, 87, 89, 91, 93, 95, 96, 98, 99, 100, 102, 105],
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
            },
            {
              label: 'Célsúly',
              data: [80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80],
              borderColor: 'rgb(75, 192, 192)',
              borderDash: [10, 5],
              borderWidth: 2,
              pointRadius: 0,
              fill: false,
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
              text: 'Súlyváltozás - A dagadék útja 🍔📈',
              font: {
                size: 16,
              },
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  return (
                    context.dataset.label + ': ' + context.parsed.y + ' kg'
                  );
                },
              },
            },
          },
          scales: {
            y: {
              beginAtZero: false,
              min: 75,
              max: 110,
              ticks: {
                callback: function (value) {
                  return value + ' kg';
                },
              },
            },
          },
        },
      });
    }
  }

  ngOnDestroy(): void {
    if (this.weightChartInstance) {
      this.weightChartInstance.destroy();
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
        next: (res) => console.log(res),
        error: (err) => console.log(err),
      });
    }
  }

  startTraining() {
    this.startWebSocketSending();
    this.training.start = new Date().toISOString();
    this.training.isActive = true;
    this.training.exerciseId = 1;

    this.homeService.startTraining(this.training).subscribe({
      next: (res) => this.getActiveTraining(),
      error: (err) => console.log(err),
    });
  }

  startWebSocketSending() {
    this.homeService.startWebSocketSending().subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  private getActiveTraining() {
    this.homeService.getActiveTraining().subscribe({
      next: (res) => {
        this.isActiveTraining = true;
        this.trainingId = res.trainingId;
        console.log(res);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  stopTraining() {
    this.stopWebSocketSending();
    this.homeService.stopTraining(this.trainingId).subscribe({
      next: (res) => {
        this.isActiveTraining = false;
        console.log(res);
      },
      error: (err) => console.log(err),
    });
  }

  stopWebSocketSending() {
    this.homeService.stopWebSocketSending().subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }
}
