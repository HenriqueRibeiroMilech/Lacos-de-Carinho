import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentService } from '../../services/payment';
import { Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cadastro',
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.css'
})
export class Cadastro {
  errorMessage = '';
  isLoading = false;
  showExistingAccountModal = false;
  existingEmail = '';

  cadastroForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  private readonly _paymentService = inject(PaymentService);
  private readonly _router = inject(Router);

  get passwordsMatch(): boolean {
    return this.cadastroForm.get('password')?.value === this.cadastroForm.get('confirmPassword')?.value;
  }

  cadastrar() {
    if (this.cadastroForm.invalid || !this.passwordsMatch) return;

    this.isLoading = true;
    this.errorMessage = '';

    // Cria preferência de pagamento - redireciona para Mercado Pago
    this._paymentService.createPaymentPreference({
      name: this.cadastroForm.get('name')?.value as string,
      email: this.cadastroForm.get('email')?.value as string,
      password: this.cadastroForm.get('password')?.value as string,
      paymentType: 'new_account'
    }).pipe(take(1)).subscribe({
      next: (response) => {
        // Salva o preferenceId para verificar depois
        localStorage.setItem('payment_preference_id', response.preferenceId);

        // Redireciona para o Mercado Pago
        window.location.href = response.checkoutUrl;
      },
      error: (error) => {
        this.isLoading = false;
        const backendError = error?.error;
        let errorMsg = '';

        if (backendError && Array.isArray(backendError.errorMessages) && backendError.errorMessages.length) {
          errorMsg = backendError.errorMessages[0];
        } else if (typeof backendError === 'string') {
          errorMsg = backendError;
        } else if (error?.status === 0) {
          errorMsg = 'Falha de conexão com o servidor.';
        } else {
          errorMsg = 'Não foi possível iniciar o pagamento.';
        }

        // Detecta se o erro é de email já existente
        if (errorMsg.toLowerCase().includes('email') &&
          (errorMsg.toLowerCase().includes('existe') ||
            errorMsg.toLowerCase().includes('cadastrado') ||
            errorMsg.toLowerCase().includes('já') ||
            errorMsg.toLowerCase().includes('registered'))) {
          this.existingEmail = this.cadastroForm.get('email')?.value || '';
          this.showExistingAccountModal = true;
        } else {
          this.errorMessage = errorMsg;
        }
      },
    });
  }

  closeModal() {
    this.showExistingAccountModal = false;
  }

  goToLogin() {
    this._router.navigate(['/entrar']);
  }
}
