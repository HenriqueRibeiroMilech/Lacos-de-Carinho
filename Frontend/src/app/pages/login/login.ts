import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user';
import { UserAuthService } from '../../services/user-auth';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
  loginErrorMessage = '';
  successMessage = '';
  returnUrl = '';

  userForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });

  private readonly _userService = inject(UserService);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _router = inject(Router);
  private readonly _route = inject(ActivatedRoute);

  ngOnInit() {
    // Captura o returnUrl da query string
    this.returnUrl = this._route.snapshot.queryParams['returnUrl'] || '';

    // Verifica se veio de um upgrade bem-sucedido
    if (this._route.snapshot.queryParams['upgradeSuccess']) {
      this.successMessage = 'Pagamento aprovado com sucesso! Por favor, faça login novamente para ativar seu plano de Organizador.';
    }
  }

  // Verifica se o usuário veio de uma lista pública (fluxo convidado)
  get isGuestFlow(): boolean {
    return this.returnUrl.includes('/lista/');
  }

  login() {
    if (this.userForm.invalid) return;

    this._userService.login(
      this.userForm.get('email')?.value as string,
      this.userForm.get('password')?.value as string).pipe(take(1)).subscribe({
        next: (response) => {
          this.loginErrorMessage = '';

          // salvar o token no localstorage
          this._userAuthService.setUserToken(response.token);

          // redirecionar para returnUrl ou painel
          if (this.returnUrl) {
            this._router.navigateByUrl(this.returnUrl);
          } else {
            this._router.navigate(['/painel']);
          }
        },
        error: (error) => {
          console.log(error);
          // Backend returns { errorMessages: string[] } with 401
          const backendError = error?.error;
          if (backendError && Array.isArray(backendError.errorMessages) && backendError.errorMessages.length) {
            this.loginErrorMessage = backendError.errorMessages[0];
          } else if (typeof backendError === 'string') {
            this.loginErrorMessage = backendError;
          } else if (error?.status === 0) {
            this.loginErrorMessage = 'Falha de conexão com o servidor.';
          } else {
            this.loginErrorMessage = 'Não foi possível realizar o login.';
          }
        },
      });
  }
}
