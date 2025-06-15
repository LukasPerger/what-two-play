import { Component } from '@angular/core';
import {HlmButtonDirective} from '@spartan-ng/helm/button';
import { cibSteam } from '@coreui/icons';
import { IconDirective } from '@coreui/icons-angular';

@Component({
  selector: 'app-landing-page',
  imports: [
    HlmButtonDirective,
    IconDirective
  ],
  templateUrl: './landing-page.html',
  styleUrl: './landing-page.css'
})
export class LandingPage {
  protected readonly icons = {cibSteam};
  loginWithSteam() {

  }
}
