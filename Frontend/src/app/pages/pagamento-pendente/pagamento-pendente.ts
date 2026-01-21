import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserAuthService } from '../../services/user-auth';
import { PaymentService } from '../../services/payment';
import { take } from 'rxjs';

@Component({
  selector: 'app-pagamento-pendente',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pagamento-pendente.html',
  styleUrl: './pagamento-pendente.css'
})
export class PagamentoPendente implements OnInit, OnDestroy {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _paymentService = inject(PaymentService);

  status: 'checking' | 'pending' | 'approved' = 'checking';
  userName: string = '';
  preferenceId: string = '';
  tokenUpdated: boolean = false;

  // Polling configuration
  private pollingInterval: any = null;
  private readonly POLLING_INTERVAL_MS = 5000; // 5 segundos
  private readonly MAX_POLLING_ATTEMPTS = 120; // 10 minutos máximo (120 * 5s)
  private pollingAttempts = 0;
  isPolling: boolean = false;

  ngOnInit() {
    // Pega o preferenceId da URL ou localStorage
    this.preferenceId = this._route.snapshot.queryParams['external_reference'] ||
      localStorage.getItem('payment_preference_id') || '';

    if (this.preferenceId) {
      this.checkPaymentStatus();
    } else {
      // Sem preferenceId, mostra como pendente (sem polling)
      this.status = 'pending';
    }
  }

  ngOnDestroy() {
    // Limpa o polling quando o componente é destruído
    this.stopPolling();
  }

  checkPaymentStatus() {
    this._paymentService.getPaymentStatus(this.preferenceId).pipe(take(1)).subscribe({
      next: (response) => {
        if (response.status === 'approved') {
          this.handleApproved(response);
        } else {
          // Ainda pendente - inicia polling
          this.status = 'pending';
          this.startPolling();
        }
      },
      error: () => {
        // Em caso de erro, mostra como pendente e inicia polling
        this.status = 'pending';
        this.startPolling();
      }
    });
  }

  private handleApproved(response: any) {
    this.status = 'approved';
    this.userName = response.name || '';
    this.stopPolling();

    // Limpa preferenceId
    localStorage.removeItem('payment_preference_id');

    // Salva novo token com role ADMIN
    if (response.token) {
      this._userAuthService.setUserToken(response.token);
      this.tokenUpdated = true;
    }
  }

  private startPolling() {
    // Não inicia se já estiver rodando ou se já foi aprovado
    if (this.pollingInterval || this.status === 'approved') {
      return;
    }

    this.isPolling = true;
    this.pollingInterval = setInterval(() => {
      this.pollingAttempts++;

      // Para o polling se atingir o máximo de tentativas
      if (this.pollingAttempts >= this.MAX_POLLING_ATTEMPTS) {
        this.stopPolling();
        return;
      }

      // Verifica o status
      this._paymentService.getPaymentStatus(this.preferenceId).pipe(take(1)).subscribe({
        next: (response) => {
          if (response.status === 'approved') {
            this.handleApproved(response);
          }
          // Se ainda pendente, continua polling
        },
        error: () => {
          // Em caso de erro, continua tentando
        }
      });
    }, this.POLLING_INTERVAL_MS);
  }

  private stopPolling() {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
      this.pollingInterval = null;
    }
    this.isPolling = false;
  }

  goToDashboard() {
    if (this.tokenUpdated) {
      window.location.href = '/painel';
    } else {
      this._router.navigate(['/painel']);
    }
  }

  logoutAndRedirect() {
    this.stopPolling();
    this._userAuthService.logout();
    this._router.navigate(['/entrar']);
  }
}
