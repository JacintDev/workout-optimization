import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { WelcomeComponent } from './welcome/welcome.component';
import { UnathorizedNavComponent } from './unathorized-nav/unathorized-nav.component';
import {
  provideHttpClient,
  HTTP_INTERCEPTORS,
  HttpClientModule,
  withInterceptors,
} from '@angular/common/http';
import { authInterceptor } from './auth.interceptor';
//material
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialogModule } from '@angular/material/dialog';
import { MatStepperModule } from '@angular/material/stepper';
//FORM
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ApplicationComponent } from './application/application.component';
import { WorkingComponent } from './working/working.component';
import { ContactComponent } from './contact/contact.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { LogoutComponent } from './logout/logout.component';
import { AuthorizedNavbarComponent } from './authorized-navbar/authorized-navbar.component';
import { HeaderComponent } from './header/header.component';
import { LogoutDialogComponent } from './logout-dialog/logout-dialog.component';
import { BicepsCurlAnimateComponent } from './biceps-curl-animate/biceps-curl-animate.component';

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent,
    UnathorizedNavComponent,
    LandingPageComponent,
    ApplicationComponent,
    WorkingComponent,
    ContactComponent,
    RegisterComponent,
    LoginComponent,
    HomeComponent,
    LogoutComponent,
    AuthorizedNavbarComponent,
    HeaderComponent,
    LogoutDialogComponent,
    BicepsCurlAnimateComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatIconModule,
    MatTabsModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatSelectModule,
    MatSnackBarModule,
    MatSidenavModule,
    MatDialogModule,
    MatStepperModule,
  ],
  providers: [provideHttpClient(withInterceptors([authInterceptor]))],
  bootstrap: [AppComponent],
})
export class AppModule {}
