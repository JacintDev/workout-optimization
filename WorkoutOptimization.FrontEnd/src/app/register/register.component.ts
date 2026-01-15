import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { RegisterModel } from '../../models/RegisterModel';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Route, Router } from '@angular/router';
import { environment } from '../../environment/environment.prod';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.sass',
})
export class RegisterComponent {
  RegisterModel: RegisterModel = new RegisterModel();
  formControl: Array<FormControl>;
  sex: any[] = [
    { value: 0, viewValue: 'Nő' },
    { value: 1, viewValue: 'Férfi' },
  ];
  http: HttpClient;
  snackbar: MatSnackBar;

  constructor(http: HttpClient, snackbar: MatSnackBar, private router: Router) {
    this.formControl = new Array<FormControl>();
    this.formControl.push(
      new FormControl('', [Validators.required, Validators.email])
    );
    this.formControl.push(
      new FormControl('', [Validators.required, Validators.minLength(4)])
    );
    this.formControl.push(
      new FormControl('', [Validators.required, Validators.minLength(2)])
    );
    this.formControl.push(
      new FormControl('', [Validators.required, Validators.minLength(2)])
    );
    this.http = http;
    this.snackbar = snackbar;
  }

  getEmailErrorMessage() {
    if (this.formControl[0].hasError('required')) {
      return 'Email cím megadása kötelező!';
    }
    return this.formControl[0].hasError('email')
      ? 'Érvénytelen email cím!'
      : '';
  }

  getPasswordErrorMessage() {
    if (this.formControl[1].hasError('required')) {
      return 'Jelszó megadása kötelező!';
    }
    return this.formControl[1].hasError('minlength')
      ? 'A jelszónak legalább 4 karakter hosszúnak kell lennie!'
      : '';
  }

  getFirstNameErrorMessage() {
    if (this.formControl[2].hasError('required')) {
      return 'Keresztnév megadása kötelező!';
    }
    return this.formControl[2].hasError('minlength')
      ? 'A keresztnévnek legalább 2 karakter hosszúnak kell lennie!'
      : '';
  }
  getLastNameErrorMessage() {
    if (this.formControl[3].hasError('required')) {
      return 'Vezetéknév megadása kötelező!';
    }
    return this.formControl[3].hasError('minlength')
      ? 'A vezetéknévnek legalább 2 karakter hosszúnak kell lennie!'
      : '';
  }

  btnCheck(): boolean {
    if (
      this.formControl[0].valid &&
      this.formControl[1].valid &&
      this.formControl[2].valid &&
      this.formControl[3].valid
    ) {
      return true;
    }
    return false;
  }
  sendRegister(): void {
    if (this.btnCheck()) {
      this.http
        .post(`${environment.apiUrl}/Auth/Register`, this.RegisterModel)
        .subscribe(
          (success) => {
            this.snackbar
              .open('Sikeres regisztráckió', 'OK', { duration: 2000 })
              .afterDismissed()
              .subscribe(() => {
                this.router.navigate(['/login']);
              });
          },
          (error) => {
            console.log(error);
          }
        );
    }
  }
}
