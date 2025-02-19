import { Component, Input, OnInit } from '@angular/core';
import { IReview } from '../../models/review.model';
import { DatePipe } from '@angular/common';
import { UUID } from 'crypto';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './review.component.html',
  styleUrl: './review.component.css'
})
export class ReviewComponent implements OnInit {

  @Input()
  review: IReview | undefined;

  stars: string[] = [];
  id: UUID = crypto.randomUUID();

  ngOnInit(){
    this.setStars();
  }

  setStars() {
    this.stars = [];
    for (let i = 1; i <= 5; i++) {
      if (i <= Math.floor(this.review?.rating ?? 0)) {
        this.stars.push('star');
      } else {
        this.stars.push('border');
      }
    }
  }
}
