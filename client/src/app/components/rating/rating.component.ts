import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { IUserRating } from '../../models/review.model';
import { UUID } from 'crypto';

@Component({
  selector: 'app-rating',
  standalone: true,
  imports: [],
  templateUrl: './rating.component.html',
  styleUrls: ['./rating.component.css']
})
export class RatingComponent implements OnInit {

  @Input() rating: IUserRating | undefined;
  @Input() size: string | undefined;

  stars: string[] = [];
  id: UUID = crypto.randomUUID();

  ngOnInit() {
    this.fillStars();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['rating']) {
      this.stars = [];
      this.fillStars();
    }
  }

  getAverageRating(): number {
    if (this.rating) {
      return this.rating.totalRating / this.rating.numberOfRatings;
    }
    return 0;
  }

  fillStars(): void {
    this.stars = [];
    const averageRating = this.getAverageRating();

    if(averageRating === 0) {
      return;
    }

    for (let i = 1; i <= 5; i++) {
      if (i <= Math.floor(averageRating)) {
        this.stars.push('star');
      } else if (i === Math.ceil(averageRating) && averageRating % 1 !== 0) {
        this.stars.push('half');
      } else {
        this.stars.push('border');
      }
    }
  }
}
