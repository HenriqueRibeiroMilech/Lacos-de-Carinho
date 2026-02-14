import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IAuthSuccessResponse } from '../interfaces/auth-success-response';
import { Observable } from 'rxjs';
import { ILoginSuccessResponse } from '../interfaces/login-success-response';
import { environment } from '../../environments/environment';

export interface IRegisterRequest {
  name: string;
  email: string;
  password: string;
  role: 'admin' | 'user';
}

export interface IRegisterResponse {
  name: string;
  token: string;
}

export interface IUpgradeResponse {
  token: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly _httpClient = inject(HttpClient);

  validateUser(): Observable<IAuthSuccessResponse> {
    return this._httpClient.get<IAuthSuccessResponse>(`${environment.apiUrl}/User`);
  }

  login(email: string, password: string): Observable<ILoginSuccessResponse> {
    const body = { email, password };
    return this._httpClient.post<ILoginSuccessResponse>(`${environment.apiUrl}/Login`, body);
  }

  register(data: IRegisterRequest): Observable<IRegisterResponse> {
    return this._httpClient.post<IRegisterResponse>(`${environment.apiUrl}/User`, data);
  }

  upgradeAccount(): Observable<IUpgradeResponse> {
    return this._httpClient.put<IUpgradeResponse>(`${environment.apiUrl}/User/upgrade`, {});
  }

  checkEmailExists(email: string): Observable<{ exists: boolean }> {
    return this._httpClient.get<{ exists: boolean }>(`${environment.apiUrl}/User/check-email/${encodeURIComponent(email)}`);
  }
}
