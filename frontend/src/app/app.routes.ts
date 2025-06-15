import {Routes} from '@angular/router';
import {LandingPage} from './pages/landing-page/landing-page';
import {BattlePage} from './pages/battle-page/battle-page';

export const routes: Routes = [
  {path: 'landing', component: LandingPage},
  {path:'battle',component:BattlePage},
  {path: '**', redirectTo: 'landing', pathMatch: 'full'},
];
