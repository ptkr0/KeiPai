import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IMessage, IMessageResponse, IMessageUser, ISendMessage } from '../models/message.model';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  private http = inject(HttpClient);
  
  constructor() { }

  getMessages(userId: string, pageNumber: number): Observable<IMessageResponse> {
    return this.http.get<IMessageResponse>(`${environment.apiUrl}/message/${userId}`);
  }

  getUsers(): Observable<IMessageUser[]> {
    return this.http.get<IMessageUser[]>(`${environment.apiUrl}/message/users`);
  }

  sendMessage(addMessage: ISendMessage): Observable<IMessage> {
    return this.http.post<IMessage>(`${environment.apiUrl}/message`, addMessage);
  }
}
