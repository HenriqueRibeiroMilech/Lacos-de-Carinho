import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PaymentForm } from '../../components/payment-form/payment-form';
import { UserAuthService } from '../../services/user-auth';
import { UserService } from '../../services/user';
import { IDirectPaymentResponse } from '../../services/payment';
import { FacebookPixelService } from '../../services/facebook-pixel';
import { take } from 'rxjs';

@Component({
  selector: 'app-cadastro',
  imports: [ReactiveFormsModule, RouterLink, CommonModule, PaymentForm],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.css'
})
export class Cadastro {
  errorMessage = '';
  showExistingAccountModal = false;
  existingEmail = '';

  // Controle de etapa: 'form' ou 'payment'
  currentStep: 'form' | 'payment' = 'form';

  cadastroForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  private readonly _router = inject(Router);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _userService = inject(UserService);
  private readonly _facebookPixelService = inject(FacebookPixelService);

  checkingEmail = false;

  get passwordsMatch(): boolean {
    return this.cadastroForm.get('password')?.value === this.cadastroForm.get('confirmPassword')?.value;
  }

  // Step 1: Valida o formulário e verifica se o email já existe
  avancarParaPagamento() {
    if (this.cadastroForm.invalid || !this.passwordsMatch) return;
    this.errorMessage = '';
    this.checkingEmail = true;

    const email = this.cadastroForm.get('email')?.value || '';

    this._userService.checkEmailExists(email).pipe(take(1)).subscribe({
      next: (response) => {
        this.checkingEmail = false;
        if (response.exists) {
          this.existingEmail = email;
          this.showExistingAccountModal = true;
        } else {
          this.currentStep = 'payment';
          this._facebookPixelService.track('Lead');
          this._facebookPixelService.track('InitiateCheckout');
        }
      },
      error: () => {
        this.checkingEmail = false;
        // Em caso de erro na verificação, avança normalmente
        this.currentStep = 'payment';
        this._facebookPixelService.track('Lead');
        this._facebookPixelService.track('InitiateCheckout');
      }
    });
  }

  // Volta do pagamento para o formulário
  voltarParaFormulario() {
    this.currentStep = 'form';
  }

  // Callback quando o pagamento é aprovado
  onPaymentApproved(response: IDirectPaymentResponse) {
    if (response.token) {
      this._userAuthService.setUserToken(response.token);
    }

    this._router.navigate(['/pagamento-sucesso'], {
      state: {
        fromCheckout: true,
        token: response.token,
        name: response.name,
        message: response.message,
        paymentMethod: response.paymentMethod || 'card'
      }
    });
  }

  // Callback quando o pagamento falha
  onPaymentError(message: string) {
    // Apenas exibe o erro no componente de pagamento
    // O componente payment-form já lida com a exibição
  }

  closeModal() {
    this.showExistingAccountModal = false;
  }

  goToLogin() {
    this._router.navigate(['/entrar']);
  }
}

