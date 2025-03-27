import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WelcomeComponent } from './welcome/welcome.component';
import { UnathorizedNavComponent } from './unathorized-nav/unathorized-nav.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ApplicationComponent } from './application/application.component';
import { WorkingComponent } from './working/working.component';
import { ContactComponent } from './contact/contact.component';

const routes: Routes = [
  { path: 'welcome', component: WelcomeComponent },
  { path: 'unauthorizednav', component: UnathorizedNavComponent },
  { path: 'landingpage', component: LandingPageComponent },
  { path: 'application', component: ApplicationComponent },
  { path: 'working', component: WorkingComponent },
  { path: 'contact', component: ContactComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
