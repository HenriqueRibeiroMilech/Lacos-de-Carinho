import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IWeddingList, IWeddingListsResponse, ICreateWeddingListRequest, IGuestDetails, IGiftItem, IRsvp, IUpsertRsvpRequest, GiftCategory } from '../interfaces/wedding';

export interface ICreateGiftItemRequest {
  name: string;
  description?: string;
  category?: GiftCategory;
}

export interface IUpdateWeddingListRequest {
  title: string;
  message?: string;
  eventDate: string;
  deliveryInfo?: string;
}

@Injectable({
  providedIn: 'root'
})
export class WeddingService {
  private readonly _httpClient = inject(HttpClient);

  // Admin endpoints
  getMyWeddingLists(): Observable<IWeddingListsResponse> {
    return this._httpClient.get<IWeddingListsResponse>(`${environment.apiUrl}/WeddingLists`);
  }

  createWeddingList(data: ICreateWeddingListRequest): Observable<IWeddingList> {
    return this._httpClient.post<IWeddingList>(`${environment.apiUrl}/WeddingLists`, data);
  }

  getWeddingListById(id: number): Observable<IWeddingList> {
    return this._httpClient.get<IWeddingList>(`${environment.apiUrl}/WeddingLists/${id}`);
  }

  updateWeddingList(id: number, data: IUpdateWeddingListRequest): Observable<IWeddingList> {
    return this._httpClient.put<IWeddingList>(`${environment.apiUrl}/WeddingLists/${id}`, data);
  }

  deleteWeddingList(id: number): Observable<void> {
    return this._httpClient.delete<void>(`${environment.apiUrl}/WeddingLists/${id}`);
  }

  // Gift Items endpoints
  addGiftItem(listId: number, data: ICreateGiftItemRequest): Observable<IGiftItem> {
    return this._httpClient.post<IGiftItem>(`${environment.apiUrl}/WeddingLists/${listId}/items`, data);
  }

  updateGiftItem(listId: number, itemId: number, data: ICreateGiftItemRequest): Observable<IGiftItem> {
    return this._httpClient.put<IGiftItem>(`${environment.apiUrl}/WeddingLists/${listId}/items/${itemId}`, data);
  }

  deleteGiftItem(listId: number, itemId: number): Observable<void> {
    return this._httpClient.delete<void>(`${environment.apiUrl}/WeddingLists/${listId}/items/${itemId}`);
  }

  // Guest endpoints
  getWeddingListByLink(shareableLink: string): Observable<IWeddingList> {
    return this._httpClient.get<IWeddingList>(`${environment.apiUrl}/Guest/lists/${shareableLink}`);
  }

  getGuestDetails(): Observable<IGuestDetails> {
    return this._httpClient.get<IGuestDetails>(`${environment.apiUrl}/Guest/me/details`);
  }

  reserveItem(itemId: number): Observable<IGiftItem> {
    return this._httpClient.post<IGiftItem>(`${environment.apiUrl}/Guest/items/${itemId}/reserve`, {});
  }

  cancelReservation(itemId: number): Observable<IGiftItem> {
    return this._httpClient.delete<IGiftItem>(`${environment.apiUrl}/Guest/items/${itemId}/reserve`);
  }

  upsertRsvp(weddingListId: number, request: IUpsertRsvpRequest): Observable<IRsvp> {
    return this._httpClient.post<IRsvp>(`${environment.apiUrl}/Guest/lists/${weddingListId}/rsvp`, request);
  }

  // PDF Report
  downloadGuestListPdf(listId: number): Observable<Blob> {
    return this._httpClient.get(`${environment.apiUrl}/WeddingLists/${listId}/guests/pdf`, {
      responseType: 'blob'
    });
  }
}
