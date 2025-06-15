import {Component, input, InputSignal} from '@angular/core';
import {
  HlmCardImports
} from '@spartan-ng/helm/card';

export interface SteamUser {
  profileImageLink: string;
  profileName: string;
  profileId: string;
}

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [
    HlmCardImports,
  ],
  templateUrl: './user-card.html',
  styleUrl: './user-card.css'
})
export class UserCard {
  public readonly user: InputSignal<SteamUser> = input.required();
}
