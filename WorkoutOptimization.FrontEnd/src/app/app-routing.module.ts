import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WelcomeComponent } from './welcome/welcome.component';
import { UnathorizedNavComponent } from './unathorized-nav/unathorized-nav.component';

const routes: Routes = [
  { path: 'welcome', component: WelcomeComponent },
  { path: 'unauthorizednav', component: UnathorizedNavComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
