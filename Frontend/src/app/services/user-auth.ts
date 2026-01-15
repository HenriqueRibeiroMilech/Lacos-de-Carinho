import { Injectable } from '@angular/core';

export type UserRole = 'CASAL' | 'CONVIDADO' | null;

// JWT payload can have claims in different formats depending on .NET configuration
export interface DecodedToken {
  exp?: number;
  iat?: number;
  nbf?: number;
  // Short claim names (when MapInboundClaims = false or custom)
  name?: string;
  role?: string;
  sid?: string;
  // Full URI claim names (default .NET behavior)
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid'?: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
  // Allow any other claims
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class UserAuthService {
  getUserToken(): string {
    return localStorage.getItem('auth-token') || '';
  }

  setUserToken(token: string): void {
    localStorage.setItem('auth-token', token);
  }

  clearUserToken(): void {
    localStorage.removeItem('auth-token');
  }

  private decodeJwtPayload(token: string): DecodedToken | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      
      const base64Url = parts[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      
      return JSON.parse(jsonPayload) as DecodedToken;
    } catch (error) {
      console.error('Error decoding JWT:', error);
      return null;
    }
  }

  getDecodedToken(): DecodedToken | null {
    const token = this.getUserToken();
    if (!token) return null;
    return this.decodeJwtPayload(token);
  }

  /**
   * Debug method - logs all claims in the token to console
   */
  debugToken(): void {
    const payload = this.getDecodedToken();
    console.log('=== JWT Token Debug ===');
    console.log('Raw token:', this.getUserToken());
    console.log('Decoded payload:', payload);
    if (payload) {
      console.log('All claims:');
      Object.entries(payload).forEach(([key, value]) => {
        console.log(`  ${key}: ${value}`);
      });
    }
    console.log('Extracted name:', this.getUserName());
    console.log('Extracted role:', this.getUserRole());
    console.log('Is admin:', this.isAdmin());
    console.log('=======================');
  }

  getTokenExpiration(): number | null {
    const payload = this.getDecodedToken();
    return payload?.exp ?? null;
  }

  isTokenExpired(): boolean {
    const exp = this.getTokenExpiration();
    if (!exp) return true;
    const nowSeconds = Math.floor(Date.now() / 1000);
    return exp <= nowSeconds;
  }

  /**
   * Gets the user's name from the token
   * Checks multiple possible claim formats
   */
  getUserName(): string | null {
    const payload = this.getDecodedToken();
    if (!payload) return null;
    
    // Try different claim name formats
    const name = 
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
      payload['name'] ||
      payload['unique_name'] ||
      payload['sub'] ||
      null;
    
    return name;
  }

  /**
   * Gets the user's role from the token
   * Checks multiple possible claim formats and normalizes to CASAL or CONVIDADO
   */
  getUserRole(): UserRole {
    const payload = this.getDecodedToken();
    if (!payload) return null;
    
    // Try different claim name formats
    const roleValue = 
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      payload['role'] ||
      null;
    
    if (!roleValue) return null;
    
    const role = roleValue.toString().toLowerCase();
    
    // Map backend roles to frontend roles
    if (role === 'admin' || role === 'casal') {
      return 'CASAL';
    }
    if (role === 'user' || role === 'convidado') {
      return 'CONVIDADO';
    }
    
    return null;
  }

  /**
   * Checks if the current user has admin (CASAL) role
   */
  isAdmin(): boolean {
    return this.getUserRole() === 'CASAL';
  }

  /**
   * Checks if the current user is a guest (CONVIDADO)
   */
  isGuest(): boolean {
    return this.getUserRole() === 'CONVIDADO';
  }

  /**
   * Checks if user is authenticated with a valid, non-expired token
   */
  isAuthenticated(): boolean {
    const token = this.getUserToken();
    if (!token) return false;
    return !this.isTokenExpired();
  }
}
