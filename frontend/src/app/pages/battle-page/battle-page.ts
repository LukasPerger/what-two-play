import {Component, model, ModelSignal} from '@angular/core';
import {SteamUser, UserCard} from './user-card/user-card';
import { HlmIconDirective } from '@spartan-ng/helm/icon';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {lucideSwords, lucideThumbsDown, lucideThumbsUp} from '@ng-icons/lucide';
import {GameCard, GameCardData} from './game-card/game-card';
import {HlmSliderImports} from '@spartan-ng/helm/slider';
import {FormsModule} from '@angular/forms';
import {HlmButtonDirective} from '@spartan-ng/helm/button';

@Component({
  selector: 'app-battle-page',
  standalone: true,
  imports: [
    UserCard,
    HlmIconDirective,
    NgIcon,
    GameCard,
    HlmSliderImports,
    FormsModule,
    HlmButtonDirective
  ],
  templateUrl: './battle-page.html',
  styleUrl: './battle-page.css',
  providers: [
    provideIcons({lucideSwords,lucideThumbsUp, lucideThumbsDown})
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
  protected gameCardData: GameCardData = {
    genres: ['Action', 'Adventure', 'RPG'],
    tags: ['Multiplayer', 'Open World', 'Co-op'],
    steamRating: 4.5
  }

  protected userRating: ModelSignal<number> = model(5);

  submitRating() {
    console.log('Bewertung abgeschickt:', this.userRating);
  }
}
