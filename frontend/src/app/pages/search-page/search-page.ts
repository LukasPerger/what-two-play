import {Component, signal} from '@angular/core';
import {HlmCommandImports} from '@spartan-ng/helm/command';
import {HlmIconDirective} from '@spartan-ng/helm/icon';
import {NgIcon, provideIcons} from '@ng-icons/core';
import {lucideSearch} from '@ng-icons/lucide';
import {FormsModule} from '@angular/forms';
import {SteamUser} from '../battle-page/user-card/user-card';
import {BrnPopoverImports} from '@spartan-ng/brain/popover';

@Component({
  selector: 'app-search-page',
  imports: [
    HlmCommandImports,
    HlmIconDirective,
    NgIcon,
    FormsModule,
    BrnPopoverImports,
    HlmCommandImports,
  ],
  templateUrl: './search-page.html',
  styleUrl: './search-page.css',
  providers: [provideIcons({lucideSearch})],
})
export class SearchPage {
  searchQuery = signal('');

  private readonly friends: SteamUser[] = [
    {
      profileImageLink: 'https://avatars.fastly.steamstatic.com/9a9bc5524e095a02dd23911cc23fb83717eb3530_full.jpg',
      profileName: 'Renschi',
      profileId: 'Damn_Thas_Crazy'
    },
    {
      profileImageLink: 'https://avatars.fastly.steamstatic.com/62f57bb5550723cbcf222df1506403879c74325c_full.jpg',
      profileName: 'Kriegerkatze123 ',
      profileId: '76561198826421951'
    },
    {
      profileImageLink: 'https://avatars.fastly.steamstatic.com/faae63457df3ed201822e8a2a151c5e97a731e5f_full.jpg',
      profileName: 'Shine_MC',
      profileId: '76561198156789012'
    },
    {
      profileImageLink: 'https://cdn.fastly.steamstatic.com/steamcommunity/public/images/items/1406990/01416ca418934ca4bb9ae088b3bbab00cf21096d.gif',
      profileName: 'FriendSheeep ',
      profileId: '76561198234567890'
    },
    {
      profileImageLink: 'https://avatars.fastly.steamstatic.com/79c93222823cba35d9b22027f298ebc6b408f4db_full.jpg',
      profileName: 'Pvt. Parts',
      profileId: '76561198312345678'
    }
  ];

  get filteredFriends() {
    const query = this.searchQuery().toLowerCase();
    return this.friends
      .filter(friend => friend.profileName.toLowerCase().includes(query))
      .slice(0, 5);
  }

}
