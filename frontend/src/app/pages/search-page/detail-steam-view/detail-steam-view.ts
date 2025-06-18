import {Component, inject, input, InputSignal} from '@angular/core';
import {
  HlmCardContentDirective,
  HlmCardDirective,
  HlmCardHeaderDirective,
  HlmCardTitleDirective
} from '@spartan-ng/helm/card';
import {HlmButtonDirective} from '@spartan-ng/helm/button';
import {FriendResponse} from '../../../../core/services/zod-types';
import {Router} from '@angular/router';

@Component({
  selector: 'app-detail-steam-view',
  imports: [
    HlmCardDirective,
    HlmCardHeaderDirective,
    HlmCardTitleDirective,
    HlmCardContentDirective,
    HlmButtonDirective
  ],
  templateUrl: './detail-steam-view.html',
  styleUrl: './detail-steam-view.css'
})
export class DetailSteamView {
  private readonly router = inject(Router);
  public readonly user: InputSignal<FriendResponse> = input.required();

  async redirectToBattle() {
    const userId = this.user().steamid;
    if (userId) {
      await this.router.navigate(['/battle', userId]);
    } else {
      console.error('User ID is not available for redirection.');
    }
  }
}
