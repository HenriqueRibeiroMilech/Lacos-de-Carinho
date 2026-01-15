import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ITemplateCategory {
  id: number;
  name: string;
}

export interface ITemplateItem {
  id: number;
  name: string;
  description?: string;
  category: ITemplateCategory;
}

export interface ITemplateGroup {
  category: ITemplateCategory;
  items: ITemplateItem[];
}

export interface ITemplateItemsResponse {
  groups: ITemplateGroup[];
}

@Injectable({
  providedIn: 'root'
})
export class TemplateItemsService {
  private readonly _httpClient = inject(HttpClient);

  getAll(): Observable<ITemplateItemsResponse> {
    return this._httpClient.get<ITemplateItemsResponse>(`${environment.apiUrl}/TemplateItems`);
  }
}
