import {Injectable} from '@angular/core';
import {ServiceBase} from './base-service';
import {firstValueFrom} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SteamService extends ServiceBase {
  protected override get controller(): string {
    return 'auth'
  }

  public async loginSteam(): Promise<void> {
    // redirect to the Steam login URL
    window.location.href = this.buildUrl('login');
  }

  public async getMySteamProfile(): Promise<string> {
    // fetch the user's Steam profile
    const response = await firstValueFrom(this.http.get<string>(this.buildUrl('me'), {
      observe: 'response',
      withCredentials: true
    }));

    if (!this.isSuccessStatusCode(response)) {
      throw new Error('Failed to fetch Steam profile');
    }

    console.log(response);
    return response.body as string;
  }
}
