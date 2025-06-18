import {Component, inject, model, ModelSignal, OnInit, Signal, signal, WritableSignal} from '@angular/core';
import {UserCard} from './user-card/user-card';
import {HlmIconDirective} from '@spartan-ng/helm/icon';
import {NgIcon, provideIcons} from '@ng-icons/core';
import {lucideSwords, lucideThumbsDown, lucideThumbsUp} from '@ng-icons/lucide';
import {GameCard} from './game-card/game-card';
import {HlmSliderImports} from '@spartan-ng/helm/slider';
import {FormsModule} from '@angular/forms';
import {HlmButtonDirective} from '@spartan-ng/helm/button';
import {SteamAuthService} from '../../../core/services/steam-auth.service';
import {SteamService} from '../../../core/services/steam.service';
import {OwnedGame, OwnedGames, PlayerSummary} from '../../../core/services/zod-types';

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
    provideIcons({lucideSwords, lucideThumbsUp, lucideThumbsDown})
  ]
})
export class BattlePage implements OnInit {
  protected readonly steamAuthService = inject(SteamAuthService);
  protected readonly steamService = inject(SteamService);

  protected user1Id: Signal<string | undefined> = signal(this.steamAuthService.getStoredProfile()?.steamId);

  protected user2Id: Signal<string | undefined> = signal(this.steamAuthService.getStoredProfile()?.steamId);

  protected user1: WritableSignal<PlayerSummary | undefined> = signal(undefined);
  protected user2: WritableSignal<PlayerSummary | undefined> = signal(undefined);

  protected currentGame: WritableSignal<OwnedGame | undefined> = signal(undefined);

  private ownedGames: OwnedGames | undefined;

  async ngOnInit() {
    const u1Id = this.user1Id();
    const u2Id = this.user2Id();
    if (!u1Id || !u2Id) {
      console.error('Steam IDs for both users are required.');
      return;
    }
    try {
      const [user1Profile, user2Profile] = await Promise.all([
        this.steamService.getUserProfile(u1Id),
        this.steamService.getUserProfile(u2Id)
      ]);

      this.ownedGames = await this.steamService.getMultiplayerApps(u1Id,u2Id);
      this.currentGame.set(this.ownedGames.games[0]);
      this.user1.set(user1Profile);
      this.user2.set(user2Profile);
    } catch (error) {
      console.error('Error fetching user profiles:', error);
    }
  }

  protected userRating: ModelSignal<number> = model(5);


  submitRating() {
    console.log('Bewertung abgeschickt:', this.userRating);
  }
}
