import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ICreatePaymentRequest {
  paymentType: 'new_account' | 'upgrade';
  name?: string;
  email?: string;
  password?: string;
}

export interface IPaymentPreferenceResponse {
  checkoutUrl: string;
  preferenceId: string;
}

export interface IPaymentStatusResponse {
  status: 'pending' | 'approved' | 'rejected' | 'cancelled' | 'not_found';
  token?: string;
  name?: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private readonly _httpClient = inject(HttpClient);

  /**
   * Cria preferência de pagamento para novo cadastro
   */
  createPaymentPreference(data: ICreatePaymentRequest): Observable<IPaymentPreferenceResponse> {
    return this._httpClient.post<IPaymentPreferenceResponse>(
      `${environment.apiUrl}/Payment/create-preference`,
      data
    );
  }

  /**
   * Cria preferência de pagamento para upgrade (usuário logado)
   */
  createUpgradePreference(): Observable<IPaymentPreferenceResponse> {
    return this._httpClient.post<IPaymentPreferenceResponse>(
      `${environment.apiUrl}/Payment/create-upgrade-preference`,
      {}
    );
  }

  /**
   * Consulta status de um pagamento
   */
  getPaymentStatus(preferenceId: string): Observable<IPaymentStatusResponse> {
    return this._httpClient.get<IPaymentStatusResponse>(
      `${environment.apiUrl}/Payment/status/${preferenceId}`
    );
  }
}
