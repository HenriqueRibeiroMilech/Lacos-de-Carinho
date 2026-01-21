import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserAuthService } from '../../services/user-auth';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  readonly userAuthService = inject(UserAuthService);
  private readonly _router = inject(Router);

  isMenuOpen = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  logout() {
    const guestListLink = this.userAuthService.getLogoutRedirectUrl();
    this.userAuthService.logout();

    if (guestListLink) {
      this._router.navigate(['/lista', guestListLink]);
    } else {
      this._router.navigate(['/entrar']);
    }
  }
}
