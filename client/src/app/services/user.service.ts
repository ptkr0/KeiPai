import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { ILogin, IRegisterInfluencer, IRegisterDeveloper, IUser } from '../models/auth.model';
import { IUserInfo } from '../models/user.model';
import { IUpdateDeveloper } from '../models/developer.model';
import { IUpdateInfluencer } from '../models/influencer.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private http = inject(HttpClient);
  // undefined - not checked, null - not logged in, IUser - logged in
  currentUserSig = signal<IUser | undefined | null>(undefined);
  user = computed(() => this.currentUserSig());

  constructor() {}

  updateUsername(username: string) {
    this.currentUserSig.update(x => x ? { ...x, username: username } : x);
  }

  getUserInfo(): Observable<IUser> {
    return this.http.get<IUser>(`${environment.apiUrl}/account/info`);
  }

  loginUser(user: ILogin): Observable<IUser> {
    return this.http.post<IUser>(`${environment.apiUrl}/account/login`, user);
  }

  registerUser(user: IRegisterInfluencer): Observable<IUser> {
    return this.http.post<IUser>(`${environment.apiUrl}/account/influencer/register`, user);
  }

  registerDeveloper(user: IRegisterDeveloper): Observable<IUser> {
    return this.http.post<IUser>(`${environment.apiUrl}/account/developer/register`, user);
  }

  logout() {
    return this.http.post(`${environment.apiUrl}/account/logout`, {});
  }

  findUser(username: string): Observable<IUserInfo> {
    return this.http.get<IUserInfo>(`${environment.apiUrl}/account/find/${username}`);
  }

  updateDeveloper(user: IUpdateDeveloper): Observable<IUpdateDeveloper> {
    return this.http.patch<IUpdateDeveloper>(`${environment.apiUrl}/account/d`, user);
  }

  updateInfluencer(user: IUpdateInfluencer): Observable<IUpdateInfluencer> {
    return this.http.patch<IUpdateInfluencer>(`${environment.apiUrl}/account/i`, user);
  }
}
