import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PaymentService } from '../../services/payment';
import { UserAuthService } from '../../services/user-auth';
import { take, interval } from 'rxjs';
import { takeWhile } from 'rxjs/operators';

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

  status: 'loading' | 'approved' | 'pending' | 'error' = 'loading';
  userName: string = '';
  message: string = '';
  preferenceId: string = '';
  checkCount: number = 0;
  maxChecks: number = 10;

  ngOnInit() {
    // Pega o preferenceId da URL (query param ou localStorage)
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
          
          // Salva o token
          if (response.token) {
            this._userAuthService.setUserToken(response.token);
          }
          
          // Limpa o preferenceId do localStorage
          localStorage.removeItem('payment_preference_id');
        } else if (response.status === 'pending') {
          this.checkCount++;
          if (this.checkCount < this.maxChecks) {
            // Tenta novamente em 3 segundos
            setTimeout(() => this.checkPaymentStatus(), 3000);
          } else {
            this.status = 'pending';
            this.message = 'Seu pagamento está sendo processado. Você receberá um email de confirmação em breve.';
          }
        } else {
          this.status = 'error';
          this.message = response.message || 'Erro ao verificar pagamento.';
        }
      },
      error: () => {
        this.status = 'error';
        this.message = 'Erro ao verificar status do pagamento.';
      }
    });
  }

  goToDashboard() {
    this._router.navigate(['/painel']);
  }

  goToCreateEvent() {
    this._router.navigate(['/criar-evento']);
  }
}
