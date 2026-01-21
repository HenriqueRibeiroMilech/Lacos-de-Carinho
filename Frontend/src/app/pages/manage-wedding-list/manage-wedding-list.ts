import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { WeddingService } from '../../services/wedding';
import { TemplateItemsService, ITemplateGroup, ITemplateItem } from '../../services/template-items';
import { IWeddingList, IGiftItem, GIFT_CATEGORIES, GiftCategory, GiftItemStatus, RsvpStatus } from '../../interfaces/wedding';
import { take } from 'rxjs';
import * as QRCode from 'qrcode';

@Component({
  selector: 'app-manage-wedding-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './manage-wedding-list.html',
  styleUrl: './manage-wedding-list.css'
})
export class ManageWeddingList implements OnInit {
  private readonly _route = inject(ActivatedRoute);
  private readonly _router = inject(Router);
  private readonly _weddingService = inject(WeddingService);
  private readonly _templateItemsService = inject(TemplateItemsService);

  // Categorias disponíveis
  readonly categories = GIFT_CATEGORIES;

  listId: number = 0;
  list: IWeddingList | null = null;
  loading = true;
  error = '';

  // Catalog
  templateGroups: ITemplateGroup[] = [];
  catalogLoading = false;
  addingItemId: number | null = null;
  selectedCategory: number | null = null;

  // My list category filter
  selectedListCategory: number | null = null;

  // Pagination for my list items
  readonly itemsPerPage = 15;
  currentListPage = 1;

  // Custom item modal
  showCustomModal = false;
  customItem = { name: '', description: '', category: GiftCategory.Outros };
  savingCustom = false;

  // PDF download
  downloadingPdf = false;

  // Share modal
  showShareModal = false;
  shareUrl = '';
  qrCodeDataUrl = '';
  linkCopied = false;

  // Settings modal
  showSettingsModal = false;
  settingsForm = { title: '', message: '', eventDate: '', deliveryInfo: '' };
  savingSettings = false;

  // Delete confirmation modal
  showDeleteModal = false;
  itemToDelete: IGiftItem | null = null;

  // Delete list modal
  showDeleteListModal = false;
  deletingList = false;

  // Tabs
  activeTab: 'gifts' | 'tracking' = 'gifts';

  ngOnInit() {
    this.listId = Number(this._route.snapshot.paramMap.get('id'));
    this.loadList();
    this.loadCatalog();
  }

  loadList() {
    this.loading = true;
    this._weddingService.getWeddingListById(this.listId).pipe(take(1)).subscribe({
      next: (response) => {
        this.list = response;
        this.shareUrl = `${window.location.origin}/lista/${response.shareableLink}`;
        this.settingsForm = {
          title: response.title || '',
          message: response.message || '',
          eventDate: response.eventDate?.split('T')[0] || '',
          deliveryInfo: response.deliveryInfo || ''
        };
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar lista';
        this.loading = false;
      }
    });
  }

  loadCatalog() {
    this.catalogLoading = true;
    this._templateItemsService.getAll().pipe(take(1)).subscribe({
      next: (response) => {
        // Use fallback if API returns empty
        if (response.groups && response.groups.length > 0) {
          this.templateGroups = response.groups;
        } else {
          this.templateGroups = this.getCompleteCatalog();
        }
        this.catalogLoading = false;
      },
      error: () => {
        // If API fails, use comprehensive fallback catalog
        this.templateGroups = this.getCompleteCatalog();
        this.catalogLoading = false;
      }
    });
  }

  private getCompleteCatalog(): ITemplateGroup[] {
    return [
      {
        category: { id: 1, name: '🍳 Cozinha' },
        items: [
          { id: 1, name: 'Jogo de Panelas Antiaderente', description: 'Conjunto completo com 5 peças de panelas antiaderentes', category: { id: 1, name: 'Cozinha' } },
          { id: 2, name: 'Liquidificador', description: 'Liquidificador de alta potência 1000W com jarra de vidro', category: { id: 1, name: 'Cozinha' } },
          { id: 3, name: 'Batedeira Planetária', description: 'Batedeira planetária com 3 batedores e tigela de inox', category: { id: 1, name: 'Cozinha' } },
          { id: 4, name: 'Air Fryer', description: 'Fritadeira elétrica sem óleo 4 litros', category: { id: 1, name: 'Cozinha' } },
          { id: 5, name: 'Cafeteira Elétrica', description: 'Cafeteira programável para 12 xícaras', category: { id: 1, name: 'Cozinha' } },
          { id: 6, name: 'Sanduicheira', description: 'Sanduicheira grill 2 em 1 antiaderente', category: { id: 1, name: 'Cozinha' } },
          { id: 7, name: 'Mixer', description: 'Mixer de mão com acessórios para triturar e bater', category: { id: 1, name: 'Cozinha' } },
          { id: 8, name: 'Processador de Alimentos', description: 'Processador multifuncional com várias lâminas', category: { id: 1, name: 'Cozinha' } },
          { id: 9, name: 'Torradeira', description: 'Torradeira elétrica para 4 fatias com controle de temperatura', category: { id: 1, name: 'Cozinha' } },
          { id: 10, name: 'Forno Elétrico', description: 'Forno elétrico de bancada 44 litros', category: { id: 1, name: 'Cozinha' } },
          { id: 11, name: 'Jogo de Talheres', description: 'Faqueiro completo em inox para 8 pessoas (48 peças)', category: { id: 1, name: 'Cozinha' } },
          { id: 12, name: 'Jogo de Pratos', description: 'Aparelho de jantar 30 peças porcelana', category: { id: 1, name: 'Cozinha' } },
          { id: 13, name: 'Jogo de Copos', description: 'Conjunto de copos para água, suco e taças de vinho', category: { id: 1, name: 'Cozinha' } },
          { id: 14, name: 'Panela de Pressão Elétrica', description: 'Panela de pressão elétrica multifuncional 6 litros', category: { id: 1, name: 'Cozinha' } },
          { id: 15, name: 'Chaleira Elétrica', description: 'Chaleira elétrica em inox 1.7 litros', category: { id: 1, name: 'Cozinha' } },
        ]
      },
      {
        category: { id: 2, name: '🛏️ Quarto' },
        items: [
          { id: 20, name: 'Jogo de Cama Queen', description: 'Jogo de cama queen 400 fios 100% algodão egípcio', category: { id: 2, name: 'Quarto' } },
          { id: 21, name: 'Jogo de Cama King', description: 'Jogo de cama king 400 fios percal', category: { id: 2, name: 'Quarto' } },
          { id: 22, name: 'Travesseiros de Pluma', description: 'Par de travesseiros de pluma de ganso', category: { id: 2, name: 'Quarto' } },
          { id: 23, name: 'Edredom Queen', description: 'Edredom queen dupla face microfibra', category: { id: 2, name: 'Quarto' } },
          { id: 24, name: 'Edredom King', description: 'Edredom king pluma sintética', category: { id: 2, name: 'Quarto' } },
          { id: 25, name: 'Manta Decorativa', description: 'Manta de sofá/cama em tricô', category: { id: 2, name: 'Quarto' } },
          { id: 26, name: 'Protetor de Colchão', description: 'Protetor impermeável para colchão queen/king', category: { id: 2, name: 'Quarto' } },
          { id: 27, name: 'Luminária de Cabeceira', description: 'Par de luminárias de mesa modernas', category: { id: 2, name: 'Quarto' } },
          { id: 28, name: 'Despertador Digital', description: 'Relógio despertador digital com projeção', category: { id: 2, name: 'Quarto' } },
        ]
      },
      {
        category: { id: 3, name: '🚿 Banheiro' },
        items: [
          { id: 30, name: 'Jogo de Toalhas Banho', description: 'Conjunto de toalhas de banho 5 peças 100% algodão', category: { id: 3, name: 'Banheiro' } },
          { id: 31, name: 'Roupões de Banho', description: 'Par de roupões de banho felpudo', category: { id: 3, name: 'Banheiro' } },
          { id: 32, name: 'Kit Organizador Banheiro', description: 'Kit de organização para bancada do banheiro', category: { id: 3, name: 'Banheiro' } },
          { id: 33, name: 'Espelho de Aumento', description: 'Espelho de aumento para maquiagem com LED', category: { id: 3, name: 'Banheiro' } },
          { id: 34, name: 'Balança Digital', description: 'Balança digital de banheiro com bioimpedância', category: { id: 3, name: 'Banheiro' } },
          { id: 35, name: 'Tapetes de Banheiro', description: 'Conjunto de tapetes antiderrapantes', category: { id: 3, name: 'Banheiro' } },
          { id: 36, name: 'Saboneteira Automática', description: 'Dispenser de sabonete automático', category: { id: 3, name: 'Banheiro' } },
        ]
      },
      {
        category: { id: 4, name: '🛋️ Sala de Estar' },
        items: [
          { id: 40, name: 'Almofadas Decorativas', description: 'Kit de 4 almofadas decorativas para sofá', category: { id: 4, name: 'Sala' } },
          { id: 41, name: 'Cortinas Blackout', description: 'Par de cortinas blackout para sala', category: { id: 4, name: 'Sala' } },
          { id: 42, name: 'Tapete Grande', description: 'Tapete de sala 2x2.5m', category: { id: 4, name: 'Sala' } },
          { id: 43, name: 'Abajur de Piso', description: 'Luminária de pé para sala de estar', category: { id: 4, name: 'Sala' } },
          { id: 44, name: 'Quadros Decorativos', description: 'Conjunto de quadros decorativos modernos', category: { id: 4, name: 'Sala' } },
          { id: 45, name: 'Vasos Decorativos', description: 'Conjunto de vasos decorativos em cerâmica', category: { id: 4, name: 'Sala' } },
          { id: 46, name: 'Relógio de Parede', description: 'Relógio de parede decorativo grande', category: { id: 4, name: 'Sala' } },
          { id: 47, name: 'Puff Organizador', description: 'Puff baú com espaço de armazenamento', category: { id: 4, name: 'Sala' } },
        ]
      },
      {
        category: { id: 5, name: '🧹 Lavanderia' },
        items: [
          { id: 50, name: 'Ferro de Passar', description: 'Ferro a vapor com base antiaderente', category: { id: 5, name: 'Lavanderia' } },
          { id: 51, name: 'Tábua de Passar', description: 'Tábua de passar roupa com suporte para ferro', category: { id: 5, name: 'Lavanderia' } },
          { id: 52, name: 'Vaporizador de Roupas', description: 'Vaporizador portátil para roupas', category: { id: 5, name: 'Lavanderia' } },
          { id: 53, name: 'Cesto de Roupa Suja', description: 'Cesto de roupa suja com divisórias', category: { id: 5, name: 'Lavanderia' } },
          { id: 54, name: 'Varal de Chão', description: 'Varal de chão dobrável com abas', category: { id: 5, name: 'Lavanderia' } },
          { id: 55, name: 'Organizador de Lavanderia', description: 'Prateleira organizadora para lavanderia', category: { id: 5, name: 'Lavanderia' } },
        ]
      },
      {
        category: { id: 6, name: '🏠 Casa Inteligente' },
        items: [
          { id: 60, name: 'Assistente Virtual', description: 'Echo Dot ou Google Nest Mini', category: { id: 6, name: 'Casa Inteligente' } },
          { id: 61, name: 'Lâmpadas Inteligentes', description: 'Kit de lâmpadas smart Wi-Fi RGB', category: { id: 6, name: 'Casa Inteligente' } },
          { id: 62, name: 'Tomadas Inteligentes', description: 'Kit de tomadas Wi-Fi com timer', category: { id: 6, name: 'Casa Inteligente' } },
          { id: 63, name: 'Câmera de Segurança', description: 'Câmera Wi-Fi interna com visão noturna', category: { id: 6, name: 'Casa Inteligente' } },
          { id: 64, name: 'Fechadura Digital', description: 'Fechadura eletrônica com senha e biometria', category: { id: 6, name: 'Casa Inteligente' } },
          { id: 65, name: 'Robô Aspirador', description: 'Aspirador robô com mapeamento inteligente', category: { id: 6, name: 'Casa Inteligente' } },
        ]
      },
      {
        category: { id: 7, name: '🍷 Mesa Posta' },
        items: [
          { id: 70, name: 'Jogo Americano', description: 'Kit de jogos americanos para 6 lugares', category: { id: 7, name: 'Mesa Posta' } },
          { id: 71, name: 'Sousplat', description: 'Conjunto de sousplats decorativos', category: { id: 7, name: 'Mesa Posta' } },
          { id: 72, name: 'Porta-Guardanapos', description: 'Conjunto de argolas porta-guardanapos', category: { id: 7, name: 'Mesa Posta' } },
          { id: 73, name: 'Fruteira', description: 'Fruteira de mesa em metal ou cerâmica', category: { id: 7, name: 'Mesa Posta' } },
          { id: 74, name: 'Balde de Gelo', description: 'Balde de gelo com pegador em inox', category: { id: 7, name: 'Mesa Posta' } },
          { id: 75, name: 'Decanter', description: 'Decanter para vinho em cristal', category: { id: 7, name: 'Mesa Posta' } },
          { id: 76, name: 'Conjunto de Xícaras', description: 'Jogo de xícaras de chá/café porcelana', category: { id: 7, name: 'Mesa Posta' } },
        ]
      },
      {
        category: { id: 8, name: '🌿 Área Externa' },
        items: [
          { id: 80, name: 'Churrasqueira Portátil', description: 'Churrasqueira a carvão portátil', category: { id: 8, name: 'Área Externa' } },
          { id: 81, name: 'Kit Churrasco', description: 'Kit de facas e utensílios para churrasco', category: { id: 8, name: 'Área Externa' } },
          { id: 82, name: 'Cadeiras de Praia', description: 'Par de cadeiras de praia reclináveis', category: { id: 8, name: 'Área Externa' } },
          { id: 83, name: 'Guarda-Sol', description: 'Guarda-sol grande com proteção UV', category: { id: 8, name: 'Área Externa' } },
          { id: 84, name: 'Caixa Térmica', description: 'Cooler térmico 42 litros', category: { id: 8, name: 'Área Externa' } },
          { id: 85, name: 'Conjunto Jardim', description: 'Mesa e cadeiras para área externa', category: { id: 8, name: 'Área Externa' } },
        ]
      },
      {
        category: { id: 9, name: '💝 Experiências' },
        items: [
          { id: 90, name: 'Jantar Romântico', description: 'Voucher para jantar em restaurante especial', category: { id: 9, name: 'Experiências' } },
          { id: 91, name: 'Spa Day', description: 'Dia de spa para o casal', category: { id: 9, name: 'Experiências' } },
          { id: 92, name: 'Passeio de Balão', description: 'Voo de balão para duas pessoas', category: { id: 9, name: 'Experiências' } },
          { id: 93, name: 'Curso de Culinária', description: 'Aula de culinária para o casal', category: { id: 9, name: 'Experiências' } },
          { id: 94, name: 'Degustação de Vinhos', description: 'Experiência de degustação em vinícola', category: { id: 9, name: 'Experiências' } },
          { id: 95, name: 'Noite no Hotel', description: 'Diária em hotel romântico', category: { id: 9, name: 'Experiências' } },
        ]
      },
      {
        category: { id: 10, name: '💰 Contribuições' },
        items: [
          { id: 100, name: 'Lua de Mel', description: 'Contribuição para viagem de lua de mel', category: { id: 10, name: 'Contribuições' } },
          { id: 101, name: 'Reforma da Casa', description: 'Contribuição para reforma do lar', category: { id: 10, name: 'Contribuições' } },
          { id: 102, name: 'Móveis Novos', description: 'Contribuição para compra de móveis', category: { id: 10, name: 'Contribuições' } },
          { id: 103, name: 'Eletrodomésticos', description: 'Contribuição para eletrodomésticos', category: { id: 10, name: 'Contribuições' } },
        ]
      }
    ];
  }

  getFilteredCatalog(): ITemplateGroup[] {
    // Get names of items already in the list
    const existingItemNames = new Set(
      this.list?.items.map(item => item.name.toLowerCase()) || []
    );

    // Filter out items that are already in the list
    let filtered = this.templateGroups.map(group => ({
      ...group,
      items: group.items.filter(item => !existingItemNames.has(item.name.toLowerCase()))
    })).filter(g => g.items.length > 0);

    // Apply category filter if selected
    if (this.selectedCategory !== null) {
      filtered = filtered.filter(g => g.category.id === this.selectedCategory);
    }

    return filtered;
  }

  // Mapeia categoria do catálogo para o ID de categoria (enum)
  private mapCatalogCategoryToId(categoryName: string): GiftCategory {
    const mapping: { [key: string]: GiftCategory } = {
      'Cozinha': GiftCategory.Cozinha,
      '🍳 Cozinha': GiftCategory.Cozinha,
      'Quarto': GiftCategory.Quarto,
      '🛏️ Quarto': GiftCategory.Quarto,
      'Banheiro': GiftCategory.Banheiro,
      '🚿 Banheiro': GiftCategory.Banheiro,
      'Sala': GiftCategory.Sala,
      'Sala de Estar': GiftCategory.Sala,
      '🛋️ Sala de Estar': GiftCategory.Sala,
      'Lavanderia': GiftCategory.Lavanderia,
      '🧹 Lavanderia': GiftCategory.Lavanderia,
      'Casa Inteligente': GiftCategory.CasaInteligente,
      '🏠 Casa Inteligente': GiftCategory.CasaInteligente,
      'Mesa Posta': GiftCategory.MesaPosta,
      '🍷 Mesa Posta': GiftCategory.MesaPosta,
      'Área Externa': GiftCategory.AreaExterna,
      '🌿 Área Externa': GiftCategory.AreaExterna,
      'Experiências': GiftCategory.Experiencias,
      '💝 Experiências': GiftCategory.Experiencias,
      'Contribuições': GiftCategory.Contribuicoes,
      '💰 Contribuições': GiftCategory.Contribuicoes
    };
    return mapping[categoryName] ?? GiftCategory.Outros;
  }

  addFromCatalog(item: ITemplateItem) {
    if (!this.list) return;
    this.addingItemId = item.id;

    // Obtém a categoria do item do catálogo
    const categoryId = this.mapCatalogCategoryToId(item.category?.name || '');

    this._weddingService.addGiftItem(this.listId, {
      name: item.name,
      description: item.description,
      category: categoryId
    }).pipe(take(1)).subscribe({
      next: (newItem) => {
        if (this.list) {
          this.list.items = [newItem, ...this.list.items];
        }
        // Item will automatically disappear from catalog via getFilteredCatalog()
        this.addingItemId = null;
      },
      error: () => {
        this.addingItemId = null;
      }
    });
  }

  addCustomItem() {
    if (!this.customItem.name.trim()) return;
    this.savingCustom = true;

    this._weddingService.addGiftItem(this.listId, {
      name: this.customItem.name,
      description: this.customItem.description,
      category: this.customItem.category
    }).pipe(take(1)).subscribe({
      next: (newItem) => {
        if (this.list) {
          this.list.items = [newItem, ...this.list.items];
        }
        this.customItem = { name: '', description: '', category: GiftCategory.Outros };
        this.showCustomModal = false;
        this.savingCustom = false;
      },
      error: () => {
        this.savingCustom = false;
      }
    });
  }

  getCategoryInfo(categoryId: number) {
    return this.categories.find(c => c.id === categoryId) || this.categories.find(c => c.id === GiftCategory.Outros)!;
  }

  getItemCategory(item: IGiftItem): number {
    return item.category ?? GiftCategory.Outros;
  }

  // Filtra itens da lista pelo filtro de categoria selecionado
  get filteredListItems(): IGiftItem[] {
    if (!this.list || !this.list.items) return [];
    if (this.selectedListCategory === null || this.selectedListCategory === undefined) return this.list.items;
    return this.list.items.filter(item => this.getItemCategory(item) === this.selectedListCategory);
  }

  // Retorna itens paginados
  get paginatedListItems(): IGiftItem[] {
    const start = (this.currentListPage - 1) * this.itemsPerPage;
    return this.filteredListItems.slice(start, start + this.itemsPerPage);
  }

  get totalListPages(): number {
    return Math.ceil(this.filteredListItems.length / this.itemsPerPage);
  }

  get showListPagination(): boolean {
    return this.filteredListItems.length > this.itemsPerPage;
  }

  goToListPage(page: number) {
    if (page >= 1 && page <= this.totalListPages) {
      this.currentListPage = page;
    }
  }

  // Reset pagination when category changes
  onListCategoryChange(categoryId: number | null) {
    this.selectedListCategory = categoryId;
    this.currentListPage = 1;
  }

  // Retorna categorias que têm itens na lista
  get listCategories() {
    if (!this.list || !this.list.items) return [];
    const categoryIds = new Set(this.list.items.map(item => this.getItemCategory(item)));
    return this.categories.filter(cat => categoryIds.has(cat.id));
  }

  // Conta itens por categoria
  getListCategoryCount(categoryId: number): number {
    if (!this.list || !this.list.items) return 0;
    return this.list.items.filter(item => this.getItemCategory(item) === categoryId).length;
  }

  deleteItem(item: IGiftItem) {
    this.itemToDelete = item;
    this.showDeleteModal = true;
  }

  cancelDelete() {
    this.showDeleteModal = false;
    this.itemToDelete = null;
  }

  confirmDelete() {
    if (!this.itemToDelete) return;

    this._weddingService.deleteGiftItem(this.listId, this.itemToDelete.id).pipe(take(1)).subscribe({
      next: () => {
        if (this.list && this.itemToDelete) {
          this.list.items = this.list.items.filter(i => i.id !== this.itemToDelete!.id);
        }
        this.cancelDelete();
      }
    });
  }

  saveSettings() {
    this.savingSettings = true;
    this._weddingService.updateWeddingList(this.listId, {
      title: this.settingsForm.title,
      message: this.settingsForm.message,
      eventDate: this.settingsForm.eventDate,
      deliveryInfo: this.settingsForm.deliveryInfo
    }).pipe(take(1)).subscribe({
      next: (response) => {
        if (this.list) {
          this.list = { ...this.list, ...response };
        }
        this.showSettingsModal = false;
        this.savingSettings = false;
      },
      error: () => {
        this.savingSettings = false;
      }
    });
  }

  deleteList() {
    this.deletingList = true;
    this._weddingService.deleteWeddingList(this.listId).pipe(take(1)).subscribe({
      next: () => {
        this.deletingList = false;
        this.showDeleteListModal = false;
        this._router.navigate(['/painel']);
      },
      error: () => {
        this.deletingList = false;
      }
    });
  }

  async openShareModal() {
    this.showShareModal = true;
    this.linkCopied = false;
    await this.generateQRCode();
  }

  async generateQRCode() {
    if (!this.shareUrl) return;
    try {
      this.qrCodeDataUrl = await QRCode.toDataURL(this.shareUrl, {
        width: 200,
        margin: 2,
        color: {
          dark: '#D97F97',
          light: '#ffffff'
        }
      });
    } catch (err) {
      console.error('Error generating QR code:', err);
    }
  }

  copyLink() {
    navigator.clipboard.writeText(this.shareUrl);
    this.linkCopied = true;
    setTimeout(() => this.linkCopied = false, 2000);
  }

  async downloadQRCode() {
    if (!this.qrCodeDataUrl) return;

    const link = document.createElement('a');
    link.download = `qrcode-${this.list?.title || 'lista'}.png`;
    link.href = this.qrCodeDataUrl;
    link.click();
  }

  shareWhatsApp() {
    const message = `💒✨ *Você está convidado(a)!* ✨💒
Olá! Temos o prazer de convidar você para celebrar conosco um momento muito especial!
🎁 Preparamos uma lista de presentes para facilitar sua escolha. Acesse o link abaixo para ver todas as opções e reservar o seu:
👉 ${this.shareUrl}
Sua presença é o nosso maior presente! 💕
_Enviado com amor através do Laços de Carinho_ 🎀`;

    const text = encodeURIComponent(message);
    window.open(`https://wa.me/?text=${text}`, '_blank');
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  getStatusLabel(status: number): string {
    const labels: Record<number, string> = {
      [GiftItemStatus.Available]: 'Disponível',
      [GiftItemStatus.Reserved]: 'Reservado',
    };
    return labels[status] || 'Desconhecido';
  }

  getStatusClass(status: number): string {
    const classes: Record<number, string> = {
      [GiftItemStatus.Available]: 'bg-green-100 text-green-600',
      [GiftItemStatus.Reserved]: 'bg-amber-100 text-amber-600',
    };
    return classes[status] || 'bg-gray-100 text-gray-600';
  }

  getRsvpStatusLabel(status: number): string {
    const labels: Record<number, string> = {
      [RsvpStatus.Pending]: 'Pendente',
      [RsvpStatus.Confirmed]: 'Confirmado',
      [RsvpStatus.Declined]: 'Não irá'
    };
    return labels[status] || 'Desconhecido';
  }

  getRsvpStatusClass(status: number): string {
    const classes: Record<number, string> = {
      [RsvpStatus.Pending]: 'bg-amber-100 text-amber-600',
      [RsvpStatus.Confirmed]: 'bg-green-100 text-green-600',
      [RsvpStatus.Declined]: 'bg-red-100 text-red-600'
    };
    return classes[status] || 'bg-gray-100 text-gray-600';
  }

  getReservedItems(): IGiftItem[] {
    return this.list?.items.filter(i => i.status === GiftItemStatus.Reserved) || [];
  }

  // Retorna lista expandida com convidado principal + acompanhantes como itens individuais
  getExpandedRsvps(): { name: string; status: number; isGuest: boolean }[] {
    if (!this.list?.rsvps) return [];

    const expanded: { name: string; status: number; isGuest: boolean }[] = [];

    for (const rsvp of this.list.rsvps) {
      // Adiciona o convidado principal
      expanded.push({
        name: rsvp.guestName || 'Convidado',
        status: rsvp.status,
        isGuest: true
      });

      // Adiciona cada acompanhante como item individual
      if (rsvp.additionalGuests && rsvp.status === RsvpStatus.Confirmed) {
        const guests = rsvp.additionalGuests.split(',').map(g => g.trim()).filter(g => g);
        for (const guest of guests) {
          expanded.push({
            name: guest,
            status: rsvp.status,
            isGuest: false
          });
        }
      }
    }

    return expanded;
  }

  // Filtra RSVPs expandidos por status
  getRsvpsByStatus(status: number): { name: string; status: number; isGuest: boolean }[] {
    return this.getExpandedRsvps().filter(r => r.status === status);
  }

  downloadGuestListPdf() {
    if (this.downloadingPdf || !this.listId) return;

    this.downloadingPdf = true;
    this._weddingService.downloadGuestListPdf(this.listId).pipe(take(1)).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `lista-convidados-${this.list?.title || 'casamento'}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.downloadingPdf = false;
      },
      error: (err) => {
        console.error('Erro ao baixar PDF:', err);
        this.downloadingPdf = false;
      }
    });
  }
}
