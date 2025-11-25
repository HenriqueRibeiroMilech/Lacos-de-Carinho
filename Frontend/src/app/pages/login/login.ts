import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user';
import { UserAuthService } from '../../services/user-auth';
import { Router } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  loginErrorMessage = '';
  userForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });

  private readonly _userService = inject(UserService);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _router = inject(Router);

  login() {
    if(this.userForm.invalid) return;

    this._userService.login(
      this.userForm.get('email')?.value as string, 
      this.userForm.get('password')?.value as string).pipe(take(1)).subscribe({
        next: (response) => {
          this.loginErrorMessage = '';
          
          // salvar o token no localstorage
          this._userAuthService.setUserToken(response.token);

          // redirecionar para tela de produtos
          this._router.navigate(['/products']);
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
