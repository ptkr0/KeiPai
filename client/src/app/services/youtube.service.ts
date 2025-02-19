import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { IGetYoutubeVideo, IVideoPage } from '../models/youtube.model';

@Injectable({
  providedIn: 'root'
})
export class YoutubeService {

  private http = inject(HttpClient);
  constructor() { }

  getVideos(userId: string, pageNumber: number): Observable<IVideoPage> {
    return this.http.get<IVideoPage>(`${environment.apiUrl}/youtube/videos/${userId}?pageNumber=${pageNumber}`);
  }

  disconnectYoutube(): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/youtube/disconnect`);
  }

  refreshYoutube(): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/youtube/refresh`, {});
  }

  getVideo(videoId: number): Observable<IGetYoutubeVideo> {
    return this.http.get<IGetYoutubeVideo>(`${environment.apiUrl}/youtube/video/${videoId}`);
  }
}
