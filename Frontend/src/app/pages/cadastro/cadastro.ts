import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user';
import { UserAuthService } from '../../services/user-auth';
import { Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-cadastro',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.css'
})
export class Cadastro {
  errorMessage = '';
  isLoading = false;
  
  cadastroForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  private readonly _userService = inject(UserService);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _router = inject(Router);

  get passwordsMatch(): boolean {
    return this.cadastroForm.get('password')?.value === this.cadastroForm.get('confirmPassword')?.value;
  }

  cadastrar() {
    if (this.cadastroForm.invalid || !this.passwordsMatch) return;

    this.isLoading = true;
    this.errorMessage = '';

    // Registro como 'admin' (CASAL) - pode criar listas
    this._userService.register({
      name: this.cadastroForm.get('name')?.value as string,
      email: this.cadastroForm.get('email')?.value as string,
      password: this.cadastroForm.get('password')?.value as string,
      role: 'admin'
    }).pipe(take(1)).subscribe({
      next: (response) => {
        this.isLoading = false;
        this._userAuthService.setUserToken(response.token);
        this._router.navigate(['/painel']);
      },
      error: (error) => {
        this.isLoading = false;
        const backendError = error?.error;
        if (backendError && Array.isArray(backendError.errorMessages) && backendError.errorMessages.length) {
          this.errorMessage = backendError.errorMessages[0];
        } else if (typeof backendError === 'string') {
          this.errorMessage = backendError;
        } else if (error?.status === 0) {
          this.errorMessage = 'Falha de conexão com o servidor.';
        } else {
          this.errorMessage = 'Não foi possível realizar o cadastro.';
        }
      },
    });
  }
}
