import {Component, computed, inject, OnInit, Signal, signal} from '@angular/core';
import {HlmCommandImports} from '@spartan-ng/helm/command';
import {HlmIconDirective} from '@spartan-ng/helm/icon';
import {NgIcon, provideIcons} from '@ng-icons/core';
import {lucideSearch} from '@ng-icons/lucide';
import {FormsModule} from '@angular/forms';
import {BrnPopoverImports} from '@spartan-ng/brain/popover';
import {DetailSteamView} from './detail-steam-view/detail-steam-view';
import {SteamService} from '../../../core/services/steam.service';
import {FriendListResponse, FriendResponse} from '../../../core/services/zod-types';

@Component({
  selector: 'app-search-page',
  imports: [
    HlmCommandImports,
    HlmIconDirective,
    NgIcon,
    FormsModule,
    BrnPopoverImports,
    HlmCommandImports,
    DetailSteamView
  ],
  templateUrl: './search-page.html',
  styleUrl: './search-page.css',
  providers: [provideIcons({lucideSearch})],
})
export class SearchPage implements OnInit {
  protected readonly _steamService = inject(SteamService);

  protected searchQuery = signal('');
  protected readonly selectedFriend = signal<FriendResponse | null>(null);
  protected readonly friends = signal<FriendListResponse | null>(null);

  protected readonly filteredFriends: Signal<FriendResponse[]> = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!this.friends()) {
      return [];
    }
    return this.friends()!.friendslist.friends.filter(friend =>
      friend.persona_name.toLowerCase().indexOf(query.toLowerCase()) !== -1
    );
  })

  async ngOnInit() {
    this.friends.set(await this._steamService.getFriends());
  }

}
