import { Component, inject, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.sass',
})
export class HomeComponent implements OnInit {
  profilePropertiesNeedSetup = true;
  user = new UserUpdateModel();

  fitnessLevel: any = [
    { value: 1, viewValue: 'Kezdő' },
    { value: 2, viewValue: 'Középhaladó' },
    { value: 3, viewValue: 'Haladó' },
  ];

  private _formBuilder = inject(FormBuilder);

  today: string = new Date().toISOString().split('T')[0]; // Ez kell majd a HTML input max attribútumhoz

  constructor(private auth: AuthService, private http: HttpClient) {}

  ngOnInit(): void {
    this.auth.currentUser$.subscribe((user) => {
      if (user?.height == null || user?.weight == null) {
        this.profilePropertiesNeedSetup = true;
      } else {
        this.profilePropertiesNeedSetup = false;
      }
    });
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
      const headers = new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.auth.getToken()}`,
      });
      this.http
        .put<any>('http://localhost:5135/Auth/UpdateUser', this.user, {
          headers,
        })
        .subscribe(
          (success) => {
            console.log(success);
          },
          (error) => {
            console.log(error);
          }
        );
    }
  }
}
