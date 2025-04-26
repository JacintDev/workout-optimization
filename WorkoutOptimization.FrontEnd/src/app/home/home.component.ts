import { Component, inject } from '@angular/core';
import { AuthService } from '../AuthService';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.sass',
})
export class HomeComponent {
  private _formBuilder = inject(FormBuilder);
  constructor(private auth: AuthService) {}
  firstFormGroup = this._formBuilder.group({
    firstCtrl: ['', Validators.required],
  });
  secondFormGroup = this._formBuilder.group({
    secondCtrl: ['', Validators.required],
  });
  isLinear = false;
}
