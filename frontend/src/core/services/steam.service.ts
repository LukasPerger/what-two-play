import {inject, Injectable} from '@angular/core';
import {ServiceBase} from './base-service';
import {
  FriendListResponse,
  friendListResponseSchema, OwnedGames, OwnedGamesResponse, ownedGamesResponseSchema,
  PlayerSummariesResponse,
  playerSummariesResponseSchema,
  PlayerSummary
} from './zod-types';
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

  public async getUserProfile(steamId: string): Promise<PlayerSummary> {
    if (!steamId) {
      throw new Error('Steam ID is required to fetch user profile');
    }

    const url = this.buildUrl(steamId);
    const response = await firstValueFrom(this.http.get(url, {observe: 'response'}));

    const result = playerSummariesResponseSchema.parse(response.body) as PlayerSummariesResponse;
    if (!this.isSuccessStatusCode(response)) {
      throw new Error('Failed to fetch user profile');
    }

    return result.response.players[0];
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

  // [HttpGet("multiplayer/{userId1:long}/{userId2:long}/apps")]
  public async getMultiplayerApps(userId1: string, userId2: string): Promise<OwnedGames> {
    if (!userId1 || !userId2) {
      throw new Error('Both user IDs are required to fetch multiplayer apps');
    }

    const url = this.buildUrl(`multiplayer/${userId1}/${userId2}/apps`);
    const response = await firstValueFrom(this.http.get(url, {observe: 'response'}));

    const result = ownedGamesResponseSchema.parse(response.body) as OwnedGamesResponse;
    if (!this.isSuccessStatusCode(response)) {
      throw new Error('Failed to fetch multiplayer apps');
    }

    return result.response;
  }

}
