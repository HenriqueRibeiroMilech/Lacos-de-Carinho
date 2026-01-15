import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _http = inject(HttpClient);

  token = '';
  newPassword = '';
  confirmPassword = '';
  loading = false;
  success = false;
  error = '';

  // Password visibility
  showPassword = false;
  showConfirmPassword = false;

  ngOnInit() {
    this.token = this._route.snapshot.queryParamMap.get('token') || '';
    
    if (!this.token) {
      this.error = 'Token inválido. Por favor, solicite um novo link de recuperação.';
    }
  }

  onSubmit() {
    this.error = '';

    // Validações
    if (!this.newPassword || this.newPassword.length < 6) {
      this.error = 'A senha deve ter pelo menos 6 caracteres';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.error = 'As senhas não coincidem';
      return;
    }

    this.loading = true;

    this._http.post(`${environment.apiUrl}/Password/reset`, {
      token: this.token,
      newPassword: this.newPassword
    }).subscribe({
      next: () => {
        this.success = true;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.message || err?.error?.errors?.[0] || 'Token inválido ou expirado. Solicite um novo link.';
        this.loading = false;
      }
    });
  }

  goToLogin() {
    this._router.navigate(['/entrar']);
  }
}
