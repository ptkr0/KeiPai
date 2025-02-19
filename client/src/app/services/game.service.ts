import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IAddCampaignGame, IGame, IGameDetail, IGamePage, IIGDB, IUpdateGame } from '../models/game.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { IUser } from '../models/auth.model';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  private http = inject(HttpClient);
  constructor() { }

  getGames(userId: string, pageNumber: number): Observable<IGamePage> {
    return this.http.get<IGamePage>(`${environment.apiUrl}/game/d/${userId}?pageNumber=${pageNumber}`);
  }

  searchKeyword(keyword: string): Observable<IIGDB[]> {
    return this.http.get<IIGDB[]>(`${environment.apiUrl}/igdb/${keyword}`);
  }

  addGame(game: FormData): Observable<IGame> {
    return this.http.post<IGame>(`${environment.apiUrl}/game`, game);
  }

  deleteGame(gameId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/game/${gameId}`);
  }

  getGame(gameId: number): Observable<IGameDetail> {
    return this.http.get<IGameDetail>(`${environment.apiUrl}/game/${gameId}`);
  }

  updateGame(game: IUpdateGame, gameId: number): Observable<IGameDetail> {
    return this.http.put<IGameDetail>(`${environment.apiUrl}/game/${gameId}`, game);
  }

  updateCover(cover: FormData, gameId: number): Observable<IGameDetail> {
    return this.http.put<IGameDetail>(`${environment.apiUrl}/game/cover/${gameId}`, cover);
  }

  deleteScreenshot(screenshotId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/game/screenshot/${screenshotId}`);
  }

  addScreenshot(screenshot: FormData, gameId: number): Observable<any> {
    return this.http.post(`${environment.apiUrl}/game/screenshot/${gameId}`, screenshot);
  }

  addCampaignGetGames(keyword: string): Observable<IAddCampaignGame[]> {
    return this.http.get<IAddCampaignGame[]>(`${environment.apiUrl}/game/find/${keyword}`);
  }
}
