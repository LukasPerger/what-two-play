import {Component, input, InputSignal} from '@angular/core';
import {
  HlmCardContentDirective,
  HlmCardDirective,
  HlmCardHeaderDirective,
  HlmCardTitleDirective
} from '@spartan-ng/helm/card';
import {HlmButtonDirective} from '@spartan-ng/helm/button';
import {FriendResponse} from '../../../../core/services/zod-types';

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
  public readonly user: InputSignal<FriendResponse> = input.required();

}
