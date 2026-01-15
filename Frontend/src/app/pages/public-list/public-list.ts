import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { take } from 'rxjs';
import { WeddingService } from '../../services/wedding';
import { UserAuthService } from '../../services/user-auth';
import { IWeddingList, IGiftItem, IRsvp, GIFT_CATEGORIES, GiftCategory, GiftItemStatus, RsvpStatus } from '../../interfaces/wedding';

@Component({
  selector: 'app-public-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './public-list.html',
  styleUrl: './public-list.css'
})
export class PublicList implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _weddingService = inject(WeddingService);
  private readonly _userAuthService = inject(UserAuthService);
  private readonly _cdr = inject(ChangeDetectorRef);

  // Categorias disponíveis
  readonly categories = GIFT_CATEGORIES;
  selectedCategory: number | null = null;

  // Pagination
  readonly itemsPerPage = 15;
  currentPage = 1;

  // Expose enums to template
  readonly RsvpStatus = RsvpStatus;
  readonly GiftItemStatus = GiftItemStatus;

  shareableLink = '';
  list: IWeddingList | null = null;
  loading = true;
  error = '';

  // RSVP state
  myRsvp: IRsvp | null = null;
  showRsvpModal = false;
  rsvpChoice: number = RsvpStatus.Confirmed;
  additionalGuestsEnabled = false;
  guestNames: string[] = [''];
  sendingRsvp = false;
  dismissedPending = false;

  // Reserve state
  reservingItemId: number | null = null;

  // Modals
  showDeliveryInfoModal = false;
  showReservedByOthersModal = false;

  get isLoggedIn(): boolean {
    return this._userAuthService.isAuthenticated();
  }

  get isGuest(): boolean {
    return this._userAuthService.isGuest();
  }

  get isOwner(): boolean {
    return this.list?.isOwner ?? false;
  }

  get reservedByMe(): IGiftItem[] {
    if (!this.list || !this.list.items) return [];
    return this.list.items.filter(i => i.myReservationId);
  }

  get availableItems(): IGiftItem[] {
    if (!this.list || !this.list.items) return [];
    let items = this.list.items.filter(i => i.status === GiftItemStatus.Available);
    
    // Filtrar por categoria se selecionada (usar !== null para permitir categoria 0)
    if (this.selectedCategory !== null) {
      items = items.filter(i => this.getItemCategory(i) === this.selectedCategory);
    }
    
    return items;
  }

  // Retorna itens paginados
  get paginatedItems(): IGiftItem[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.availableItems.slice(start, start + this.itemsPerPage);
  }

  get totalPages(): number {
    return Math.ceil(this.availableItems.length / this.itemsPerPage);
  }

  get showPagination(): boolean {
    return this.availableItems.length > this.itemsPerPage;
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      // Scroll to top of gift section
      const section = document.querySelector('[data-gift-section]');
      if (section) {
        section.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }
  }

  // Reset pagination when category changes
  onCategoryChange(categoryId: number | null) {
    this.selectedCategory = categoryId;
    this.currentPage = 1;
  }

  get reservedByOthers(): IGiftItem[] {
    if (!this.list || !this.list.items) return [];
    return this.list.items.filter(i => i.status === GiftItemStatus.Reserved && !i.myReservationId);
  }

  // Extrai a categoria do item (usa campo category do backend)
  getItemCategory(item: IGiftItem): number {
    return item.category ?? GiftCategory.Outros;
  }

  // Retorna o nome do item (agora não precisa mais remover prefixo)
  getItemDisplayName(item: IGiftItem): string {
    return item.name;
  }

  // Retorna informações da categoria
  getCategoryInfo(categoryId: number) {
    return this.categories.find(c => c.id === categoryId) || this.categories.find(c => c.id === GiftCategory.Outros)!;
  }

  // Conta quantos itens disponíveis há em cada categoria
  getCategoryCount(categoryId: number): number {
    if (!this.list || !this.list.items) return 0;
    return this.list.items.filter(i => 
      i.status === GiftItemStatus.Available && this.getItemCategory(i) === categoryId
    ).length;
  }

  // Retorna categorias que têm itens disponíveis
  get availableCategories() {
    return this.categories.filter(cat => this.getCategoryCount(cat.id) > 0);
  }

  ngOnInit() {
    this._route.paramMap.subscribe(params => {
      this.shareableLink = params.get('link') || '';
      
      // Se não estiver logado, redireciona para login com returnUrl
      if (!this.isLoggedIn) {
        this._router.navigate(['/entrar'], {
          queryParams: { returnUrl: `/lista/${this.shareableLink}` }
        });
        return;
      }
      
      if (this.shareableLink) {
        this.loadList();
      }
    });
  }

  loadList() {
    if (!this.shareableLink) {
      this.error = 'Link inválido';
      this.loading = false;
      this._cdr.markForCheck();
      return;
    }

    this.loading = true;
    this.error = '';
    this._cdr.markForCheck();

    this._weddingService.getWeddingListByLink(this.shareableLink).pipe(take(1)).subscribe({
      next: (response) => {
        this.list = response;
        this.loading = false;
        
        if (this.list) {
             if (this.list.isOwner) {
               // Owner preview mode - set a demo RSVP for visualization
               this.myRsvp = { id: 0, guestId: 0, status: RsvpStatus.Pending };
             } else {
               // Guest mode - load real RSVP
               this.loadGuestDetails();
             }
        } else {
             this.error = 'Lista não encontrada';
        }
        this._cdr.markForCheck();
      },
      error: (err) => {
        this.error = err?.error?.message || 'Lista não encontrada';
        this.loading = false;
        this._cdr.markForCheck();
      }
    });
  }

  loadGuestDetails() {
    this._weddingService.getGuestDetails().pipe(take(1)).subscribe({
      next: (details) => {
        const event = details.events.find(e => e.weddingList.shareableLink === this.shareableLink);
        if (event) {
          this.myRsvp = event.rsvp;
        } else {
          // Primeiro acesso - ainda não tem RSVP
          this.myRsvp = { id: 0, guestId: 0, status: RsvpStatus.Pending };
        }

        // Auto open RSVP modal se ainda não confirmou presença
        if (this.myRsvp?.status === RsvpStatus.Pending && !this.dismissedPending) {
          setTimeout(() => this.openRsvpModal(), 300);
        }
        this._cdr.markForCheck();
      },
      error: () => {
        // Se der erro, assume que é primeiro acesso
        this.myRsvp = { id: 0, guestId: 0, status: RsvpStatus.Pending };
        setTimeout(() => this.openRsvpModal(), 300);
        this._cdr.markForCheck();
      }
    });
  }

  formatEventDate(): string {
    if (!this.list?.eventDate) return '';
    try {
      const date = new Date(this.list.eventDate + 'T00:00:00');
      return date.toLocaleDateString('pt-BR', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
        year: 'numeric'
      });
    } catch {
      return this.list.eventDate;
    }
  }

  // RSVP methods
  openRsvpModal() {
    this.showRsvpModal = true;
    this.additionalGuestsEnabled = !!this.myRsvp?.additionalGuests;
    this.guestNames = this.myRsvp?.additionalGuests ? this.myRsvp.additionalGuests.split(/,\s*/) : [''];
    this.rsvpChoice = this.myRsvp?.status ?? RsvpStatus.Confirmed;
  }

  closeRsvpModal() {
    this.showRsvpModal = false;
    if (this.rsvpChoice === RsvpStatus.Pending) {
      this.dismissedPending = true;
    }
  }

  addGuest() {
    this.guestNames.push('');
  }

  removeGuest(index: number) {
    this.guestNames = this.guestNames.filter((_, i) => i !== index);
  }

  serializeAdditionalGuests(): string | undefined {
    const cleaned = this.guestNames.map(g => g.trim()).filter(g => g.length > 0);
    if (this.additionalGuestsEnabled && cleaned.length) return cleaned.join(', ');
    return undefined;
  }

  submitRsvp() {
    if (!this.list) return;
    
    // Owner preview mode - update local state only (no API call)
    if (this.isOwner) {
      const additionalGuests = this.rsvpChoice === RsvpStatus.Confirmed ? this.serializeAdditionalGuests() : undefined;
      this.myRsvp = { 
        id: 0, 
        guestId: 0, 
        status: this.rsvpChoice,
        additionalGuests 
      };
      this.showRsvpModal = false;
      this._cdr.markForCheck();
      return;
    }
    
    this.sendingRsvp = true;
    const additionalGuests = this.rsvpChoice === RsvpStatus.Confirmed ? this.serializeAdditionalGuests() : undefined;

    this._weddingService.upsertRsvp(this.list.id, {
      status: this.rsvpChoice,
      additionalGuests
    }).pipe(take(1)).subscribe({
      next: (rsvp) => {
        this.myRsvp = rsvp;
        this.sendingRsvp = false;
        this.showRsvpModal = false;
        if (this.rsvpChoice === RsvpStatus.Pending) {
          this.dismissedPending = true;
        }
      },
      error: () => {
        this.sendingRsvp = false;
      }
    });
  }

  // Reserve methods
  reserveItem(item: IGiftItem) {
    if (!this.isLoggedIn) {
      // Redirect to login with return URL
      this._router.navigate(['/entrar'], { 
        queryParams: { returnUrl: `/lista/${this.shareableLink}` }
      });
      return;
    }

    // Owner preview mode - update local state only (no API call)
    if (this.isOwner) {
      item.status = GiftItemStatus.Reserved;
      item.myReservationId = item.id; // Mark as reserved by "me"
      this._cdr.markForCheck();
      return;
    }

    this.reservingItemId = item.id;
    this._weddingService.reserveItem(item.id).pipe(take(1)).subscribe({
      next: () => {
        this.reservingItemId = null;
        this.loadList();
      },
      error: () => {
        this.reservingItemId = null;
      }
    });
  }

  cancelReservation(item: IGiftItem) {
    if (!item.myReservationId) return;

    // Owner preview mode - update local state only (no API call)
    if (this.isOwner) {
      item.status = GiftItemStatus.Available;
      item.myReservationId = undefined;
      this._cdr.markForCheck();
      return;
    }
    
    this._weddingService.cancelReservation(item.id).pipe(take(1)).subscribe({
      next: () => {
        this.loadList();
      }
    });
  }

  goToLogin() {
    this._router.navigate(['/entrar'], {
      queryParams: { returnUrl: `/lista/${this.shareableLink}` }
    });
  }

  goToRegister() {
    this._router.navigate(['/register'], {
      queryParams: { returnUrl: `/lista/${this.shareableLink}` }
    });
  }
}
