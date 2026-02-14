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

// --- Checkout Transparente ---

export interface IDirectPaymentRequest {
  paymentType: 'new_account' | 'upgrade';
  token?: string;          // Token do cartão (gerado pelo SDK)
  paymentMethodId: string; // ex: "visa", "master", "pix"
  issuerId: string;        // ID do emissor
  installments: number;    // Parcelas
  payerEmail: string;      // Email do pagador
  name?: string;           // Nome (para novo cadastro)
  password?: string;       // Senha (para novo cadastro)
}

export interface IDirectPaymentResponse {
  status: string;
  statusDetail: string;
  message: string;
  token?: string;
  name?: string;
  paymentId: number;
  pixQrCode?: string;
  pixQrCodeBase64?: string;
  ticketUrl?: string;
  paymentMethod?: string; // 'pix' | 'card' — set by the payment-form component
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private readonly _httpClient = inject(HttpClient);

  /**
   * Processa pagamento direto via Checkout Transparente
   */
  processDirectPayment(data: IDirectPaymentRequest): Observable<IDirectPaymentResponse> {
    return this._httpClient.post<IDirectPaymentResponse>(
      `${environment.apiUrl}/Payment/process`,
      data
    );
  }

  /**
   * Cria preferência de pagamento para novo cadastro (Checkout Pro - fallback)
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

  /**
   * Verifica status de pagamento Pix (polling)
   */
  checkPixStatus(mpPaymentId: number): Observable<IDirectPaymentResponse> {
    return this._httpClient.get<IDirectPaymentResponse>(
      `${environment.apiUrl}/Payment/check-pix/${mpPaymentId}`
    );
  }
}

