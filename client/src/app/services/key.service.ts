import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { IKeyForm, IKeyPagination } from '../models/key.model';

@Injectable({
  providedIn: 'root'
})
export class KeyService {

  private http = inject(HttpClient);
  constructor() { }

  getKeys(gameId: number, pageNumber: number, platforms?: string[]): Observable<IKeyPagination> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString());
  
    if (platforms && platforms.length > 0) {
      platforms.forEach(platform => {
        params = params.append('platforms', platform);
      });
    }
  
    return this.http.get<IKeyPagination>(`${environment.apiUrl}/key/${gameId}`, { params });
  }

  addKeys(gameId: number, platformId: number, keys: IKeyForm[]): Observable<any> {
    return this.http.post(`${environment.apiUrl}/key/`, { gameId, platformId, keys });
  }

  deleteKeys(keyIds: number[]): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/key/`, { body: { keyIds } });
  }
}
