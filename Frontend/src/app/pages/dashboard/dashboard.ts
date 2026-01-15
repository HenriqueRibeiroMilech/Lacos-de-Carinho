import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserAuthService } from '../../services/user-auth';
import { UserService } from '../../services/user';
import { WeddingService } from '../../services/wedding';
import { IWeddingListShort, IGuestDetails, IGuestEvent, RsvpStatus, ListType, LIST_TYPE_LABELS } from '../../interfaces/wedding';
import { take } from 'rxjs';
import * as QRCode from 'qrcode';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _userService = inject(UserService);
  private readonly _weddingService = inject(WeddingService);
  private readonly _router = inject(Router);

  // Expose enum to template
  readonly RsvpStatus = RsvpStatus;
  readonly ListType = ListType;
  readonly LIST_TYPE_LABELS = LIST_TYPE_LABELS;

  userName: string = '';
  isAdmin: boolean = false;
  
  // Admin data
  myWeddingLists: IWeddingListShort[] = [];
  loadingMyLists: boolean = false;

  // Guest data (for all users)
  guestDetails: IGuestDetails | null = null;
  loadingGuestEvents: boolean = false;

  // Share modal
  showShareModal: boolean = false;
  selectedList: IWeddingListShort | null = null;
  shareUrl: string = '';
  qrCodeDataUrl: string = '';
  linkCopied: boolean = false;

  // Upgrade modal
  showUpgradeModal: boolean = false;
  upgrading: boolean = false;
  upgradeError: string = '';

  // Computed property to check if user has guest events
  get hasGuestEvents(): boolean {
    return !!(this.guestDetails && this.guestDetails.events && this.guestDetails.events.length > 0);
  }

  ngOnInit() {
    // Debug: log token info to console
    this._userAuthService.debugToken();
    
    this.userName = this._userAuthService.getUserName() || 'Usuário';
    this.isAdmin = this._userAuthService.isAdmin();
    
    console.log('Dashboard - userName:', this.userName, 'isAdmin:', this.isAdmin);
    
    // Load admin lists if admin
    if (this.isAdmin) {
      this.loadMyWeddingLists();
    }
    
    // Load guest events for all users
    this.loadGuestEvents();
  }

  loadMyWeddingLists() {
    this.loadingMyLists = true;
    this._weddingService.getMyWeddingLists().pipe(take(1)).subscribe({
      next: (response) => {
        this.myWeddingLists = response.lists || [];
        this.loadingMyLists = false;
      },
      error: () => {
        this.loadingMyLists = false;
        this.myWeddingLists = [];
      }
    });
  }

  loadGuestEvents() {
    this.loadingGuestEvents = true;
    this._weddingService.getGuestDetails().pipe(take(1)).subscribe({
      next: (response) => {
        this.guestDetails = response;
        this.loadingGuestEvents = false;
      },
      error: () => {
        this.loadingGuestEvents = false;
        this.guestDetails = null;
      }
    });
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric' 
    });
  }

  getItemsCount(list: IWeddingListShort): { total: number; reserved: number } {
    return { 
      total: list.totalItems || 0, 
      reserved: list.reservedItems || 0 
    };
  }

  getRsvpCount(list: IWeddingListShort): { confirmed: number; total: number } {
    return { 
      total: list.totalRsvps || 0, 
      confirmed: list.confirmedRsvps || 0 
    };
  }

  copyLink(link: string) {
    const fullLink = `${window.location.origin}/lista/${link}`;
    navigator.clipboard.writeText(fullLink);
    // TODO: Add toast notification
  }

  logout() {
    this._userAuthService.clearUserToken();
    this._router.navigate(['/entrar']);
  }

  navigateToCreateEvent() {
    this._router.navigate(['/criar-evento']);
  }

  navigateToEvent(id: number) {
    this._router.navigate(['/gerenciar-lista', id]);
  }

  // Guest event methods
  getGuestStatusLabel(status: string): string {
    switch (status) {
      case 'Confirmed': return 'Presença confirmada';
      case 'Pending': return 'Pendente';
      case 'Declined': return 'Não irá';
      default: return status;
    }
  }

  getGuestStatusClass(status: string): string {
    switch (status) {
      case 'Confirmed': return 'text-green-600 bg-green-100';
      case 'Pending': return 'text-amber-600 bg-amber-100';
      case 'Declined': return 'text-red-600 bg-red-100';
      default: return 'text-gray-600 bg-gray-100';
    }
  }

  navigateToGuestList(event: IGuestEvent) {
    this._router.navigate(['/lista', event.weddingList.shareableLink]);
  }

  openUpgradeInfo() {
    this.showUpgradeModal = true;
    this.upgradeError = '';
  }

  closeUpgradeModal() {
    this.showUpgradeModal = false;
    this.upgradeError = '';
  }

  confirmUpgrade() {
    this.upgrading = true;
    this.upgradeError = '';

    this._userService.upgradeAccount().pipe(take(1)).subscribe({
      next: (response) => {
        // Salva o novo token com a role atualizada
        this._userAuthService.setUserToken(response.token);
        
        this.upgrading = false;
        this.showUpgradeModal = false;
        
        // Atualiza estado local
        this.isAdmin = true;
        
        // Carrega as listas (agora que é admin)
        this.loadMyWeddingLists();
        
        // Redireciona para criar evento
        this._router.navigate(['/criar-evento']);
      },
      error: (err) => {
        this.upgrading = false;
        this.upgradeError = err.error?.errorMessages?.[0] || 'Erro ao fazer upgrade. Tente novamente.';
      }
    });
  }

  // Share modal methods
  openShareModal(list: IWeddingListShort) {
    this.selectedList = list;
    this.shareUrl = `${window.location.origin}/lista/${list.shareableLink}`;
    this.showShareModal = true;
    this.linkCopied = false;
    this.generateQRCode();
  }

  closeShareModal() {
    this.showShareModal = false;
    this.selectedList = null;
    this.qrCodeDataUrl = '';
  }

  async generateQRCode() {
    if (!this.shareUrl) return;
    try {
      this.qrCodeDataUrl = await QRCode.toDataURL(this.shareUrl, {
        width: 200,
        margin: 1,
        color: { dark: '#D97F97', light: '#FFFFFF' }
      });
    } catch (err) {
      console.error('Error generating QR code:', err);
    }
  }

  copyShareLink() {
    navigator.clipboard.writeText(this.shareUrl);
    this.linkCopied = true;
    setTimeout(() => this.linkCopied = false, 2000);
  }

  shareWhatsApp() {
    const text = encodeURIComponent(`Confira nossa lista de presentes: ${this.shareUrl}`);
    window.open(`https://wa.me/?text=${text}`, '_blank');
  }

  downloadQRCode() {
    if (!this.qrCodeDataUrl) return;
    const link = document.createElement('a');
    link.download = `qrcode-${this.selectedList?.title || 'lista'}.png`;
    link.href = this.qrCodeDataUrl;
    link.click();
  }

  shareNative() {
    if (navigator.share) {
      navigator.share({
        title: this.selectedList?.title || 'Lista de Presentes',
        text: 'Confira nossa lista de presentes!',
        url: this.shareUrl
      }).catch(() => {});
    } else {
      this.copyShareLink();
    }
  }

  encodeURIComponent(str: string): string {
    return encodeURIComponent(str);
  }
}
