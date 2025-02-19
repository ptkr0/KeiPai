import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IInfluencerInfo } from '../models/influencer.model';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class InfluencerService {

  private http = inject(HttpClient);
  constructor() { }

  getInfluencerInfo(): Observable<IInfluencerInfo> {
    return this.http.get<IInfluencerInfo>(`${environment.apiUrl}/account/influencer/info`,);
  }
}
