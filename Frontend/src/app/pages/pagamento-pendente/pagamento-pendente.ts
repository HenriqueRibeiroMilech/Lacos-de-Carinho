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
  message: string = '';
  preferenceId: string = '';
  tokenUpdated: boolean = false;

  // Polling para Pix (Checkout Transparente)
  private pixPaymentId: number | null = null;
  private pollingInterval: any = null;
  pixPollingActive = false;

  // Timer elapsed
  elapsedSeconds = 0;
  private timerInterval: any = null;

  get elapsedDisplay(): string {
    const mins = Math.floor(this.elapsedSeconds / 60);
    const secs = this.elapsedSeconds % 60;
    if (mins > 0) {
      return `${mins}m ${secs.toString().padStart(2, '0')}s`;
    }
    return `${secs}s`;
  }

  ngOnInit() {
    const navState = history.state;

    if (navState?.fromCheckout && navState?.paymentId) {
      this.status = 'pending';
      this.message = navState.message || 'Seu pagamento Pix está sendo processado.';
      this.pixPaymentId = navState.paymentId;
      this.startPixPolling();
      this.startTimer();
      return;
    }

    // Fluxo Checkout Pro (fallback)
    this.preferenceId = this._route.snapshot.queryParams['external_reference'] ||
      localStorage.getItem('payment_preference_id') || '';

    if (this.preferenceId) {
      this.checkPaymentStatus();
    } else {
      this.status = 'pending';
      this.message = 'Seu pagamento está sendo processado. Assim que for confirmado, você receberá um email.';
    }
  }

  ngOnDestroy() {
    this.stopPixPolling();
    this.stopTimer();
  }

  private startTimer() {
    this.elapsedSeconds = 0;
    this.timerInterval = setInterval(() => {
      this.elapsedSeconds++;
    }, 1000);
  }

  private stopTimer() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }

  private startPixPolling() {
    if (!this.pixPaymentId) return;
    this.pixPollingActive = true;
    let attempts = 0;
    const maxAttempts = 360;

    this.pollingInterval = setInterval(() => {
      attempts++;
      if (attempts > maxAttempts) {
        this.stopPixPolling();
        this.stopTimer();
        this.message = 'Pix expirado. Por favor, tente novamente.';
        return;
      }

      this._paymentService.checkPixStatus(this.pixPaymentId!)
        .pipe(take(1))
        .subscribe({
          next: (response) => {
            if (response.status === 'approved') {
              this.stopPixPolling();
              this.stopTimer();

              if (response.token) {
                this._userAuthService.setUserToken(response.token);
              }

              this._router.navigate(['/pagamento-sucesso'], {
                state: {
                  fromCheckout: true,
                  token: response.token,
                  name: response.name,
                  message: response.message,
                  paymentMethod: 'pix'
                }
              });
            } else if (response.status === 'rejected') {
              this.stopPixPolling();
              this.stopTimer();
              this._router.navigate(['/pagamento-falha'], {
                state: {
                  fromCheckout: true,
                  message: response.message || 'Pagamento rejeitado.'
                }
              });
            }
          },
          error: () => {
            // Silently ignore polling errors
          }
        });
    }, 5000);
  }

  private stopPixPolling() {
    this.pixPollingActive = false;
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
      this.pollingInterval = null;
    }
  }

  checkPaymentStatus() {
    this._paymentService.getPaymentStatus(this.preferenceId).pipe(take(1)).subscribe({
      next: (response) => {
        if (response.status === 'approved') {
          this.status = 'approved';
          this.userName = response.name || '';

          localStorage.removeItem('payment_preference_id');

          if (response.token) {
            this._userAuthService.setUserToken(response.token);
            this.tokenUpdated = true;
          }
        } else {
          this.status = 'pending';
          this.message = 'Seu pagamento está sendo processado. Assim que for confirmado, você receberá um email.';
        }
      },
      error: () => {
        this.status = 'pending';
        this.message = 'Seu pagamento está sendo processado. Assim que for confirmado, você receberá um email.';
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
