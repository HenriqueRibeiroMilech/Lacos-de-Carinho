import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PaymentService } from '../../services/payment';
import { UserAuthService } from '../../services/user-auth';
import { FacebookPixelService } from '../../services/facebook-pixel';
import { take } from 'rxjs';

@Component({
  selector: 'app-pagamento-sucesso',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pagamento-sucesso.html',
  styleUrl: './pagamento-sucesso.css'
})
export class PagamentoSucesso implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _paymentService = inject(PaymentService);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _facebookPixelService = inject(FacebookPixelService);

  status: 'loading' | 'approved' | 'pending' | 'error' = 'loading';
  userName: string = '';
  message: string = '';
  preferenceId: string = '';
  checkCount: number = 0;
  maxChecks: number = 60;
  tokenUpdated: boolean = false;

  // Detalhes do pagamento para exibir
  paymentMethod: string = '';
  paymentValue: string = 'R$ 39,90';

  ngOnInit() {
    // Verifica se veio do Checkout Transparente (navigation state)
    const navState = history.state;

    if (navState?.fromCheckout) {
      this.status = 'approved';
      this.userName = navState.name || '';
      this.message = navState.message || 'Pagamento aprovado! Bem-vindo ao Laços de Carinho.';
      this.tokenUpdated = !!navState.token;
      this.paymentMethod = navState.paymentMethod === 'pix' ? 'Pix' : 'Cartão de Crédito';
      this._facebookPixelService.track('Purchase', { value: 39.90, currency: 'BRL' });
      return;
    }

    // Fluxo Checkout Pro (fallback): verifica via preferenceId
    this.preferenceId = this._route.snapshot.queryParams['external_reference'] ||
      localStorage.getItem('payment_preference_id') || '';

    if (!this.preferenceId) {
      this.status = 'error';
      this.message = 'Referência do pagamento não encontrada.';
      return;
    }

    this.checkPaymentStatus();
  }

  checkPaymentStatus() {
    this._paymentService.getPaymentStatus(this.preferenceId).pipe(take(1)).subscribe({
      next: (response) => {
        if (response.status === 'approved') {
          this.status = 'approved';
          this.userName = response.name || '';
          this.message = response.message;

          localStorage.removeItem('payment_preference_id');

          if (response.token) {
            this._userAuthService.setUserToken(response.token);
            this.tokenUpdated = true;
          }
          this._facebookPixelService.track('Purchase', { value: 39.90, currency: 'BRL' });
        } else if (response.status === 'pending') {
          this.checkCount++;
          if (this.checkCount < this.maxChecks) {
            const delay = Math.min(2000 + (this.checkCount * 300), 5000);
            this.message = 'Seu pagamento está sendo processado. Assim que for confirmado, você receberá um email.';
            setTimeout(() => this.checkPaymentStatus(), delay);
          } else {
            this.status = 'pending';
            this.message = 'Seu pagamento está sendo processado. Assim que for confirmado, você receberá um email.';
          }
        } else {
          this.status = 'error';
          this.message = response.message || 'Erro ao verificar pagamento.';
        }
      },
      error: () => {
        this.checkCount++;
        if (this.checkCount < this.maxChecks) {
          setTimeout(() => this.checkPaymentStatus(), 3000);
        } else {
          this.status = 'error';
          this.message = 'Erro ao verificar status do pagamento. Tente atualizar a página.';
        }
      }
    });
  }

  goToDashboard() {
    if (this.tokenUpdated) {
      window.location.href = '/painel';
    } else {
      this._router.navigate(['/painel']);
    }
  }

  goToCreateEvent() {
    if (this.tokenUpdated) {
      window.location.href = '/criar-evento';
    } else {
      this._router.navigate(['/criar-evento']);
    }
  }

  logoutAndRedirect() {
    this._userAuthService.logout();
    this._router.navigate(['/entrar']);
  }
}
