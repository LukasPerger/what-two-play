import { Component } from '@angular/core';
import {SteamUser, UserCard} from './user-card/user-card';
import { HlmIconDirective } from '@spartan-ng/helm/icon';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {lucideSwords} from '@ng-icons/lucide';

@Component({
  selector: 'app-battle-page',
  standalone: true,
  imports: [
    UserCard,
    HlmIconDirective,
    NgIcon
  ],
  templateUrl: './battle-page.html',
  styleUrl: './battle-page.css',
  providers: [
    provideIcons({lucideSwords})
  ]
})
export class BattlePage {

  protected user1 : SteamUser = {
    profileImageLink: 'https://avatars.fastly.steamstatic.com/9a9bc5524e095a02dd23911cc23fb83717eb3530_full.jpg',
    profileName: 'Renschi',
    profileId: 'Damn_Thas_Crazy'
  }

  protected user2 : SteamUser = {
    profileImageLink: 'https://avatars.fastly.steamstatic.com/62f57bb5550723cbcf222df1506403879c74325c_full.jpg',
    profileName: 'Kriegerkatze123 ',
    profileId: '76561198826421951'
  }

}
