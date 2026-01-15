import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user';
import { UserAuthService } from '../../services/user-auth';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements OnInit {
  errorMessage = '';
  isLoading = false;
  returnUrl = '';
  
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
