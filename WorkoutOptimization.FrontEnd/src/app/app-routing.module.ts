import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WelcomeComponent } from './welcome/welcome.component';
import { UnathorizedNavComponent } from './unathorized-nav/unathorized-nav.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ApplicationComponent } from './application/application.component';
import { WorkingComponent } from './working/working.component';
import { ContactComponent } from './contact/contact.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './auth.guard';
import { GuestGuard } from './guest.guard';

const routes: Routes = [
  { path: 'welcome', component: WelcomeComponent },
  { path: 'unauthorizednav', component: UnathorizedNavComponent },
  { path: 'landingpage', component: LandingPageComponent },
  { path: 'application', component: ApplicationComponent },
  { path: 'working', component: WorkingComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'register', component: RegisterComponent, canActivate: [GuestGuard] },
  { path: 'login', component: LoginComponent, canActivate: [GuestGuard] },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [AuthGuard],
    data: { roles: ['User', 'Admin'] },
  },
  { path: '**', redirectTo: 'welcome', pathMatch: 'full' }, // Redirect to welcome for any unknown routes
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
