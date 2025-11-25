import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserAuthService } from '../../services/user-auth';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  private readonly _auth = inject(UserAuthService);
  private readonly _router = inject(Router);

  logout() {
    this._auth.clearUserToken();
    this._router.navigate(['/login']);
  }
}
