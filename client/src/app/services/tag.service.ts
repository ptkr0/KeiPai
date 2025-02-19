import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ITag } from '../models/tag.model';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class TagService {

  private http = inject(HttpClient);
  constructor() { }

  getTags(): Observable<ITag[]> {
    return this.http.get<ITag[]>(`${environment.apiUrl}/tag`);
  }
}
