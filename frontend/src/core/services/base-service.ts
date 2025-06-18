import { Directive, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Directive()
export abstract class ServiceBase {
  private static readonly baseUrl = "http://localhost:5032/api";
  protected readonly http: HttpClient = inject(HttpClient);

  protected abstract get controller(): string;

  protected isSuccessStatusCode(response: { status: number }): boolean {
    return response.status >= 200 && response.status < 300;
  }

  protected buildUrl(action: string | null, ...queryParams: ((QueryParam | null) | undefined)[]): string {
    let url = `${ServiceBase.baseUrl}/${this.controller}`;
    if (action !== null) {
      url = `${url}/${action}`;
    }

    if (queryParams.length > 0 && queryParams.some(x => x != undefined)) {
      let first = true;
      for (const [key, value] of queryParams.filter(p => p != null && p[0] != null)
        .map(p => p as QueryParam)) {
        let prefix = "&";
        if (first) {
          first = false;
          prefix = "?";
        }
        url = url.concat(`${prefix}${key}=${value?.toString()}`);
      }
    }

    return url;
  }
}

export type QueryParam = [string, any];
