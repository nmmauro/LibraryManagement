import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review } from '../models/review';
import { environment } from '../core/environment';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  constructor(private http: HttpClient) { }

  addReview(bookId: number, review: Partial<Review>): Observable<Review> {
    console.log(`Sending review to API: ${environment.apiUrl}/books/${bookId}/reviews`, review);
    return this.http.post<Review>(`${environment.apiUrl}/books/${bookId}/reviews`, review);
  }

  getBookReviews(bookId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`${environment.apiUrl}/books/${bookId}/reviews`);
  }
}