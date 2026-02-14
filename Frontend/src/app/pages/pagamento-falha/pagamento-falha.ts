import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { UserAuthService } from '../../services/user-auth';

@Component({
  selector: 'app-pagamento-falha',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pagamento-falha.html',
  styleUrl: './pagamento-falha.css'
})
export class PagamentoFalha implements OnInit {
  private readonly _router = inject(Router);
  private readonly _userAuthService = inject(UserAuthService);

  errorMessage = '';

  ngOnInit() {
    const navState = history.state;
    if (navState?.fromCheckout && navState?.message) {
      this.errorMessage = navState.message;
    }
  }

  logoutAndRedirect() {
    this._userAuthService.logout();
    this._router.navigate(['/entrar']);
  }

  tryAgain() {
    // Se estiver logado, volta para o painel (fluxo de upgrade)
    if (this._userAuthService.isAuthenticated()) {
      this._router.navigate(['/painel']);
    } else {
      // Se não estiver logado, volta para o cadastro (fluxo de novo usuário)
      this._router.navigate(['/cadastro']);
    }
  }
}
