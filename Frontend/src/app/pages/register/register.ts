import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user';
import { UserAuthService } from '../../services/user-auth';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements OnInit {
  errorMessage = '';
  isLoading = false;
  returnUrl = '';
  showExistingAccountModal = false;
  existingEmail = '';

  registerForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  private readonly _userService = inject(UserService);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _router = inject(Router);
  private readonly _route = inject(ActivatedRoute);

  ngOnInit() {
    this.returnUrl = this._route.snapshot.queryParams['returnUrl'] || '';
  }

  get passwordsMatch(): boolean {
    return this.registerForm.get('password')?.value === this.registerForm.get('confirmPassword')?.value;
  }

  register() {
    if (this.registerForm.invalid || !this.passwordsMatch) return;

    this.isLoading = true;
    this.errorMessage = '';

    // Registro sempre como 'user' (FREE). Upgrade para PRO vem depois.
    this._userService.register({
      name: this.registerForm.get('name')?.value as string,
      email: this.registerForm.get('email')?.value as string,
      password: this.registerForm.get('password')?.value as string,
      role: 'user'
    }).pipe(take(1)).subscribe({
      next: (response) => {
        this.isLoading = false;
        this._userAuthService.setUserToken(response.token);

        if (this.returnUrl) {
          this._router.navigateByUrl(this.returnUrl);
        } else {
          this._router.navigate(['/painel']);
        }
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
          errorMsg = 'Não foi possível realizar o cadastro.';
        }

        // Detecta se o erro é de email já existente
        // Aceita "e-mail" ou "email", e textos como "já está em uso", "já existe", "já cadastrado"
        const lowerError = errorMsg.toLowerCase().replace(/-/g, ''); // remove hífens para normalizar e-mail -> email

        if (lowerError.includes('email') &&
          (lowerError.includes('existe') ||
            lowerError.includes('cadastrado') ||
            lowerError.includes('ja') || // 'já' sem acento
            lowerError.includes('uso') || // "já está em uso"
            lowerError.includes('registered'))) {
          this.existingEmail = this.registerForm.get('email')?.value || '';
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
    const loginUrl = this.returnUrl
      ? `/entrar?returnUrl=${encodeURIComponent(this.returnUrl)}`
      : '/entrar';
    this._router.navigateByUrl(loginUrl);
  }
}
