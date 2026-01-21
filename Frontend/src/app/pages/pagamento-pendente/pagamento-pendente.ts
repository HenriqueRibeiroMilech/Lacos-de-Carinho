import { Component, inject, OnInit } from '@angular/core';
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
export class PagamentoPendente implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _paymentService = inject(PaymentService);

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
    } else {
      // Sem preferenceId, mostra como pendente
      this.status = 'pending';
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
