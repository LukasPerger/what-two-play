import {Component, inject} from '@angular/core';
import {HlmButtonDirective} from '@spartan-ng/helm/button';
import {cibSteam} from '@coreui/icons';
import {IconDirective} from '@coreui/icons-angular';
import {SteamAuthService} from '../../../core/services/steam-auth.service';

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
  protected readonly steamService = inject(SteamAuthService);
  protected readonly icons = {cibSteam};

  async loginWithSteam() {
    try {
      await this.steamService.loginSteam();
    } catch (error) {
      console.error('Login failed:', error);
    }
  }

  async getMySteamProfile() {
    try {
      const profile = await this.steamService.getMySteamProfile();
      console.log('My Steam Profile:', profile);
    } catch (error) {
      console.error('Failed to fetch Steam profile:', error);
    }
  }
}
