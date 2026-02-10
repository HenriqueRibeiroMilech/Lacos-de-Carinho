export interface IWeddingList {
  id: number;
  title: string;
  message?: string;
  eventDate: string;
  shareableLink: string;
  deliveryInfo?: string;
  listType: ListType;
  isOwner?: boolean;
  items: IGiftItem[];
  rsvps?: IRsvp[];  // Optional - may not be returned by backend
}

// Short version returned by GetAll
export interface IWeddingListShort {
  id: number;
  title: string;
  shareableLink: string;
  eventDate?: string;
  message?: string;
  listType: ListType;
  totalItems: number;
  reservedItems: number;
  totalRsvps: number;
  confirmedRsvps: number;
}

// ListType enum - must match backend
export enum ListType {
  Wedding = 0,
  BridalShower = 1
}

// Display labels for list types
export const LIST_TYPE_LABELS: Record<ListType, string> = {
  [ListType.Wedding]: 'Casamento',
  [ListType.BridalShower]: 'Chá de Panela'
};

export interface IGiftItem {
  id: number;
  name: string;
  description?: string;
  category?: number; // GiftCategory enum value from backend
  status: number; // GiftItemStatus enum: 0 = Available, 1 = Reserved
  reservedByName?: string;
  myReservationId?: number; // ID da reserva do convidado logado (se houver)
}

// GiftItemStatus enum - must match backend
export enum GiftItemStatus {
  Available = 0,
  Reserved = 1
}

// GiftCategory enum - must match backend Ldc.Communication.Enums.GiftCategory
export enum GiftCategory {
  Outros = 0,
  Cozinha = 1,
  Quarto = 2,
  Banheiro = 3,
  Sala = 4,
  Lavanderia = 5,
  CasaInteligente = 6,
  MesaPosta = 7,
  AreaExterna = 8,
  Escritorio = 9,
  BarELazer = 10
}

// Categorias disponíveis para presentes (UI display)
export const GIFT_CATEGORIES = [
  { id: GiftCategory.Cozinha, name: 'Cozinha', icon: '🍳' },
  { id: GiftCategory.Quarto, name: 'Quarto', icon: '🛏️' },
  { id: GiftCategory.Banheiro, name: 'Banheiro', icon: '🚿' },
  { id: GiftCategory.Sala, name: 'Sala de Estar', icon: '🛋️' },
  { id: GiftCategory.Lavanderia, name: 'Lavanderia', icon: '🧹' },
  { id: GiftCategory.CasaInteligente, name: 'Casa Inteligente', icon: '🏠' },
  { id: GiftCategory.MesaPosta, name: 'Mesa Posta', icon: '🍷' },
  { id: GiftCategory.AreaExterna, name: 'Área Externa', icon: '🌿' },
  { id: GiftCategory.Escritorio, name: 'Escritório', icon: '💻' },
  { id: GiftCategory.BarELazer, name: 'Bar e Lazer', icon: '🍸' },
  { id: GiftCategory.Outros, name: 'Outros', icon: '📦' }
] as const;

// RsvpStatus enum - must match backend
export enum RsvpStatus {
  Pending = 0,
  Confirmed = 1,
  Declined = 2
}

export interface IRsvp {
  id: number;
  guestId: number;
  guestName?: string;
  status: number; // RsvpStatus enum
  additionalGuests?: string;
}

export interface IWeddingListsResponse {
  lists: IWeddingListShort[];
}

export interface ICreateWeddingListRequest {
  title: string;
  message?: string;
  eventDate: string;
  deliveryInfo?: string;
  listType: ListType;
}

// Guest interfaces
export interface IGuestDetails {
  userId: number;
  events: IGuestEvent[];
}

export interface IGuestEvent {
  rsvp: IRsvp;
  weddingList: IWeddingListShort;
}

export interface IUpsertRsvpRequest {
  status: number; // RsvpStatus enum
  additionalGuests?: string;
}
