import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserAuthService } from '../../services/user-auth';
import { PaymentService } from '../../services/payment';
import { SignalRService } from '../../services/signalr';
import { take, Subscription } from 'rxjs';

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
  private readonly _signalRService = inject(SignalRService);

  private signalRSub?: Subscription;

  status: 'checking' | 'pending' | 'approved' = 'checking';
  userName: string = '';
  preferenceId: string = '';
  tokenUpdated: boolean = false;

  ngOnInit() {
    // Pega o preferenceId da URL ou localStorage
    this.preferenceId = this._route.snapshot.queryParams['external_reference'] ||
      localStorage.getItem('payment_preference_id') || '';

    if (this.preferenceId) {
      this.checkPaymentStatus();
      this.connectToSignalR();
    } else {
      // Sem preferenceId, mostra como pendente
      this.status = 'pending';
    }
  }

  ngOnDestroy() {
    // Limpa a conexão SignalR ao sair
    this.signalRSub?.unsubscribe();
    this._signalRService.disconnect();
  }

  /**
   * Conecta ao SignalR para receber notificações em tempo real
   */
  private async connectToSignalR() {
    try {
      await this._signalRService.connectToPaymentHub(this.preferenceId);

      // Ouve eventos de pagamento aprovado
      this.signalRSub = this._signalRService.onPaymentApproved.subscribe(event => {
        console.log('PagamentoPendente: Received payment approved event', event);

        this.status = 'approved';
        this.userName = event.userName || '';

        // Salva o novo token com role ADMIN
        if (event.token) {
          this._userAuthService.setUserToken(event.token);
          this.tokenUpdated = true;
        }
      });
    } catch (err) {
      console.error('PagamentoPendente: Failed to connect to SignalR', err);
      // Não faz nada se falhar - ainda tem a verificação via API
    }
  }

  checkPaymentStatus() {
    this._paymentService.getPaymentStatus(this.preferenceId).pipe(take(1)).subscribe({
      next: (response) => {
        if (response.status === 'approved') {
          this.status = 'approved';
          this.userName = response.name || '';

          // Limpa preferenceId
          localStorage.removeItem('payment_preference_id');

          // Salva novo token com role ADMIN
          if (response.token) {
            this._userAuthService.setUserToken(response.token);
            this.tokenUpdated = true;
          }
        } else {
          // Ainda pendente ou outro status
          this.status = 'pending';
        }
      },
      error: () => {
        // Em caso de erro, mostra como pendente
        this.status = 'pending';
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

  logoutAndRedirect() {
    this._userAuthService.logout();
    this._router.navigate(['/entrar']);
  }
}
