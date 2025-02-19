import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IVideoPage } from '../models/youtube.model';
import { IConnectOtherMedia, IOtherMedia } from '../models/otherMedia.model';

@Injectable({
  providedIn: 'root'
})
export class OtherMediaService {

  private http = inject(HttpClient);
  constructor() { }

  getContent(userId: string, pageNumber: number): Observable<IOtherMedia[]> {
    return this.http.get<IOtherMedia[]>(`${environment.apiUrl}/othermedia/content/${userId}`);
  }

  connectMedia(form: IConnectOtherMedia): Observable<IConnectOtherMedia> {
    return this.http.post<IConnectOtherMedia>(`${environment.apiUrl}/othermedia/connect`, form);
  }

  disconnectMedia(): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/othermedia/disconnect`);
  }

  addContent(form: FormData): Observable<IOtherMedia> {
    return this.http.post<IOtherMedia>(`${environment.apiUrl}/othermedia/add`, form);
  }

  getContentById(mediaId: number): Observable<IOtherMedia> {
    return this.http.get<IOtherMedia>(`${environment.apiUrl}/othermedia/content/s/${mediaId}`);
  }
}
