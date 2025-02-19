import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { IActiveCampaign, IAddCampaign, ICampaign, ICampaignDetail, ICampaignPage, ICampaignStats, ICanRequest } from '../models/campaign.model';

@Injectable({
  providedIn: 'root'
})
export class CampaignService {

  private http = inject(HttpClient);
  constructor() { }

  getCampaignsForDev(pageNumber: number): Observable<ICampaignPage>{
    return this.http.get<ICampaignPage>(`${environment.apiUrl}/campaign/d?pageNumber=${pageNumber}`,);
  }

  getActiveCampaignsForDev(): Observable<IActiveCampaign[]>{
    return this.http.get<IActiveCampaign[]>(`${environment.apiUrl}/campaign/d/active`);
  }


  getCampaignsForInfluencer(pageNumber: number, userId?: string, platforms?: string[], tags?: string[], includeComingSoon?: boolean): Observable<ICampaignPage>{
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString());

    if (platforms && platforms.length > 0) {
      platforms.forEach(platform => {
        params = params.append('platforms', platform);
      });
    }

    if (tags && tags.length > 0) {
      tags.forEach(tag => {
        params = params.append('tags', tag);
      });
    }

    if (userId) {
      params = params.append('userId', userId);
    }

    if (includeComingSoon) {
      params = params.append('includeComingSoon', includeComingSoon.toString());
    }
    
    return this.http.get<ICampaignPage>(`${environment.apiUrl}/campaign/i`, { params });
  }

  addCampaign(campaign: IAddCampaign): Observable<ICampaign>{
    return this.http.post<ICampaign>(`${environment.apiUrl}/campaign/`, campaign);
  }

  closeCampaign(campaignId: number): Observable<void>{
    return this.http.patch<void>(`${environment.apiUrl}/campaign/${campaignId}`, null);
  }

  getCampaign(campaignId: number): Observable<ICampaignDetail>{
    return this.http.get<ICampaignDetail>(`${environment.apiUrl}/campaign/${campaignId}`);
  }

  checkIfCanJoin(campaignId: number): Observable<ICanRequest>{
    return this.http.get<ICanRequest>(`${environment.apiUrl}/campaign/c/${campaignId}`);
  }

  getCampaignStats(campaignId: number): Observable<ICampaignStats>{
    return this.http.get<ICampaignStats>(`${environment.apiUrl}/campaign/stats/${campaignId}`);
  }
}
