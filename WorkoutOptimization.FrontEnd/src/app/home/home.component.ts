import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { AuthService } from '../AuthService';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.sass',
})
export class HomeComponent implements OnInit {
  profilePropertiesNeedSetup = true; //
  fitnessLevel: any = [
    { value: 1, viewValue: 'Kezdő' },
    { value: 2, viewValue: 'Középhaladó' },
    { value: 3, viewValue: 'Haladó' },
  ];

  private _formBuilder = inject(FormBuilder);
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
  firstFormGroup = this._formBuilder.group({
    firstCtrl: [
      '',
      [Validators.required, Validators.min(12), Validators.max(100)],
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
      //TODO: send update data to the backend server
    }
  }
}
