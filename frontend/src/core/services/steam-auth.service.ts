import {Injectable} from '@angular/core';
import {ServiceBase} from './base-service';
import {firstValueFrom} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SteamAuthService extends ServiceBase {
  private readonly STORAGE_KEY = 'steam_profile';

  protected override get controller(): string {
    return 'auth'
  }


  public async loginSteam(): Promise<void> {
    // redirect to the Steam login URL
    window.location.href = this.buildUrl('login');
  }

  public async getMySteamProfile(): Promise<SteamProfile> {
    const cachedProfile = this.getStoredProfile();
    if (cachedProfile) {
      return cachedProfile;
    }

    const response = await firstValueFrom(this.http.get<string>(this.buildUrl('me'), {
      observe: 'response',
      withCredentials: true
    }));

    if (!this.isSuccessStatusCode(response)) {
      throw new Error('Failed to fetch Steam profile');
    }

    const responseBody = response.body as SteamProfile | null;

    if (!responseBody) {
      throw new Error('No Steam profile data received');
    }

    this.storeProfile(responseBody);
    return responseBody;
  }

  public async logoutSteam(): Promise<void> {
    localStorage.removeItem(this.STORAGE_KEY);
    // redirect to the Steam logout URL
    window.location.href = this.buildUrl('logout');
  }

  private storeProfile(profile: SteamProfile): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(profile));
  }

  public getStoredProfile(): SteamProfile | null {
    const stored = localStorage.getItem(this.STORAGE_KEY);
    return stored ? JSON.parse(stored) : null;
  }
}

export interface SteamProfile {
  username: string;
  steamId: string;
}
