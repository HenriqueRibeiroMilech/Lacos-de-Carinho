import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserAuthService {
  getUserToken() {
    return localStorage.getItem('auth-token') || '';
  }

  setUserToken(token: string) {
    localStorage.setItem('auth-token', token);
  }

  clearUserToken() {
    localStorage.removeItem('auth-token');
  }

  private decodeJwtPart<T = any>(base64Url: string): T | null {
    try {
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(atob(base64).split('').map(c => {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      return JSON.parse(json) as T;
    } catch {
      return null;
    }
  }

  getTokenExpiration(): number | null {
    const token = this.getUserToken();
    if (!token) return null;
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    const payload = this.decodeJwtPart<{ exp?: number }>(parts[1]);
    return payload?.exp ?? null;
  }

  isTokenExpired(): boolean {
    const exp = this.getTokenExpiration();
    if (!exp) return true;
    const nowSeconds = Math.floor(Date.now() / 1000);
    return exp <= nowSeconds;
  }
}
