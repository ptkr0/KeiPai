import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IGetTwitchStream, IStreamPage } from '../models/twitch.model';

@Injectable({
  providedIn: 'root'
})
export class TwitchService {

  private http = inject(HttpClient);
  constructor() { }

  disconnectTwitch(): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/twitch/disconnect`);
  }

  getStreams(userId: string, pageNumber: number): Observable<IStreamPage> {
    return this.http.get<IStreamPage>(`${environment.apiUrl}/twitch/streams/${userId}?pageNumber=${pageNumber}`);
  }

  getStream(streamId: number): Observable<IGetTwitchStream> {
    return this.http.get<IGetTwitchStream>(`${environment.apiUrl}/twitch/stream/${streamId}`);
  }
}
