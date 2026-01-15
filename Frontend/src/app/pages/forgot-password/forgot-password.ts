import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {
  private readonly _http = inject(HttpClient);

  email = '';
  loading = false;
  submitted = false;
  error = '';

  onSubmit() {
    if (!this.email.trim()) {
      this.error = 'Por favor, digite seu email';
      return;
    }

    this.loading = true;
    this.error = '';

    this._http.post(`${environment.apiUrl}/Password/request-reset`, { email: this.email })
      .subscribe({
        next: () => {
          this.submitted = true;
          this.loading = false;
        },
        error: () => {
          // Mesmo em caso de erro, mostramos mensagem genérica por segurança
          this.submitted = true;
          this.loading = false;
        }
      });
  }
}
