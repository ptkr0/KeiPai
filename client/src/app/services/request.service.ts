import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IRequestDeveloper, IRequestInfluencer, ISendRequest, ISendRequestResponse } from '../models/request.model';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class RequestService {

  private http = inject(HttpClient);
  constructor() { }

  sendRequest(request: ISendRequest): Observable<ISendRequestResponse> {
    return this.http.post<ISendRequestResponse>(`${environment.apiUrl}/request`, request);
  }

  getRequestsForInfluencer(option: string): Observable<IRequestInfluencer[]>{
    return this.http.get<IRequestInfluencer[]>(`${environment.apiUrl}/request/i?option=${option}`);
  }

  getRequestsForDeveloper(campaignId?: number[], option?: string): Observable<IRequestDeveloper[]>{
    let params = new HttpParams()

    if (campaignId) {
      params = params.append('campaignId', campaignId.toString());
    }

    if (option) {
      params = params.append('option', option);
    }

    return this.http.get<IRequestDeveloper[]>(`${environment.apiUrl}/request/d`, { params });
  }

  cancelRequest(requestId: number): Observable<void>{
    return this.http.delete<void>(`${environment.apiUrl}/request?requestId=${requestId}`);
  }

  handleRequest(requestId: number, decision: number): Observable<void>{
    return this.http.patch<void>(`${environment.apiUrl}/request/${requestId}`, decision);
  }
}
