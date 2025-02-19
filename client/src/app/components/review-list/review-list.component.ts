import { Component, inject, Input } from '@angular/core';
import { IOtherMedia } from '../../models/otherMedia.model';
import { OtherMediaService } from '../../services/other-media.service';
import { ReviewService } from '../../services/review.service';
import { IReview } from '../../models/review.model';
import { ReviewComponent } from "../review/review.component";
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-review-list',
  standalone: true,
  imports: [ReviewComponent],
  templateUrl: './review-list.component.html',
  styleUrl: './review-list.component.css'
})
export class ReviewListComponent {
  @Input()
  userId: string | undefined;

  reviews: IReview[] = [];

  reviewService = inject(ReviewService);
  userService = inject(UserService);

  ngOnInit() {
    this.fetchReviews();
  }

  fetchReviews() {
    if (this.userId) {
      this.reviewService.getReviews(this.userId).subscribe({
        next: (res) => {
          this.reviews = res;
          console.log('Reviews:', res);
        },
        error: (error) => {
          console.error('Error fetching reviews:', error);
        }
      });
    }
  }
}
