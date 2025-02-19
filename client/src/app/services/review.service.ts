import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IAddReview, IReview, IUpdateReview } from '../models/review.model';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  private http = inject(HttpClient);
  constructor() { }

  getReviews(userId: string): Observable<IReview[]>{
    return this.http.get<IReview[]>(`${environment.apiUrl}/review/${userId}`);
  }

  getReviewForUser(userId: string): Observable<IReview>{ 
    return this.http.get<IReview>(`${environment.apiUrl}/review/user/${userId}`);
  }

  deleteReview(revieweeId: string): Observable<void>{
    return this.http.delete<void>(`${environment.apiUrl}/review/${revieweeId}`);
  }

  addReview(review: IAddReview): Observable<IReview>{
    return this.http.post<IReview>(`${environment.apiUrl}/review`, review);
  }

  updateReview(review: IAddReview): Observable<IUpdateReview>{
    return this.http.put<IUpdateReview>(`${environment.apiUrl}/review`, review);
  }
}
