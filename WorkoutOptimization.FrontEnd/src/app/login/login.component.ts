import { Component } from '@angular/core';
import { LoginModel } from '../../models/LoginModel';
import { FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { TokenModel } from '../../models/TokenModel';
import { Route, Router } from '@angular/router';
import { AuthService } from '../AuthService';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environment/environment';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.sass',
})
export class LoginComponent {
  LoginModel: LoginModel = new LoginModel();
  formControl: Array<FormControl>;
  sex: any[] = [
    { value: 0, viewValue: 'Nő' },
    { value: 1, viewValue: 'Férfi' },
  ];
  http: HttpClient;

  constructor(
    http: HttpClient,
    private router: Router,
    private authService: AuthService,
    private snackbar: MatSnackBar
  ) {
    this.formControl = new Array<FormControl>();
    this.formControl.push(
      new FormControl('', [Validators.required, Validators.email])
    );
    this.formControl.push(
      new FormControl('', [Validators.required, Validators.minLength(4)])
    );

    this.http = http;
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

  btnCheck(): boolean {
    if (this.formControl[0].valid && this.formControl[1].valid) {
      return true;
    }
    return false;
  }

  sendLogin(): void {
    if (this.btnCheck()) {
      this.http
        .post<TokenModel>(`${environment.apiUrl}Auth/Login`, this.LoginModel)
        .subscribe(
          (resp) => {
            localStorage.setItem('token', resp.token);
            localStorage.setItem('expiration', resp.expiration.toString());
            this.router.navigate(['/home']);
          },
          (error) => {
            this.snackbar.open('Sikertelen bejelentkezés!', 'OK', {
              duration: 2000,
            });
          }
        );
    }
  }
}
