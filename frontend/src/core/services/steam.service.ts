import {inject, Injectable} from '@angular/core';
import {ServiceBase} from './base-service';
import {FriendListResponse, friendListResponseSchema} from './zod-types';
import {SteamAuthService} from './steam-auth.service';
import {firstValueFrom} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SteamService extends ServiceBase {

  private readonly steamAuthService = inject(SteamAuthService);

  protected override get controller(): string {
    return 'steam'
  }


  public async getFriends(): Promise<FriendListResponse> {
    const steamId = this.steamAuthService.getStoredProfile()?.steamId;

    if (!steamId) {
      throw new Error('Steam ID is not available. Please log in first.');
    }

    const url = this.buildUrl(steamId + '/friends');
    const response = await firstValueFrom(this.http.get(url, {observe: 'response'}));


    if (!this.isSuccessStatusCode(response)) {
      throw new Error('Failed to fetch friends list');
    }

    const responseBody = friendListResponseSchema.parse(response.body) as FriendListResponse;
    if (!responseBody || !responseBody.friendslist) {
      throw new Error('No friends data received');
    }

    return responseBody;
  }

}
