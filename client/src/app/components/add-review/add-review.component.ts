import { Component, EventEmitter, inject, Input, Output, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { IAddReview, IReview } from '../../models/review.model';
import { ReviewService } from '../../services/review.service';
import { ConfirmationDialogService } from '../confirmation-dialog/confirmation-dialog.service';
import { on } from 'events';

export interface AddReviewDialogResponse {
  review: IAddReview;
  mode: 'add' | 'edit' | 'delete';
}


@Component({
  selector: 'app-add-review',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './add-review.component.html',
  styleUrl: './add-review.component.css'
})
export class AddReviewComponent {
  @Input() visible = false;
  @Input() userWasRated: boolean = false;
  @Input() userId: string = '';

  @Output() confirm = new EventEmitter<AddReviewDialogResponse>();
  @Output() cancel = new EventEmitter<void>();

  reviewForm: FormGroup;
  currentReview: IReview | undefined = undefined;
  note: string = 'Great Experience';
  loading = true;

  reviewService = inject(ReviewService);
  confirmationDialogService = inject(ConfirmationDialogService);

  constructor(private fb: FormBuilder) {
    this.reviewForm = this.fb.group({
      rating: new FormControl([5]),
      comment: new FormControl('', [Validators.maxLength(250)]),
      isAnonymous: new FormControl(false)
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && changes['visible'].currentValue === true) {
      this.reviewForm.reset({
      });
      this.fetchReview();
    }
  }

  onRatingChange(rating: number) {
    this.reviewForm.get('rating')?.patchValue(rating);
    this.setNote();
    console.log('Rating:', rating);
  }

  fetchReview() {
    if (!this.userWasRated) {
      this.loading = false;
      return;
    }
    this.reviewService.getReviewForUser(this.userId).subscribe((review) => {
      this.currentReview = review;
      this.reviewForm.patchValue({
        rating: review.rating,
        comment: review.comment,
        isAnonymous: review.isAnonymous
      });
      this.setNote();
      this.loading = false;
    });
  }

  setNote() {
    switch (this.reviewForm.get('rating')?.value) {
      case 1:
        this.note = 'Very Poor Experience';
        break;
      case 2:
        this.note = 'Poor Experience';
        break;
      case 3:
        this.note = 'Fair Experience';
        break;
      case 4:
        this.note = 'Good Experience';
        break;
      case 5:
        this.note = 'Great Experience';
        break;
    }
  }

  onSubmit() {
    if (this.reviewForm.valid) {
      const review: IAddReview = {
        revieweeId: this.userId,
        rating: this.reviewForm.get('rating')?.value ?? 5,
        comment: this.reviewForm.get('comment')?.value ?? '',
        isAnonymous: this.reviewForm.get('isAnonymous')?.value ?? false
      };
      const mode = this.userWasRated ? 'edit' : 'add';
      this.confirm.emit({review, mode});
      this.visible = false;
    }
  }

  onCancel() {
    this.cancel.emit();
    this.visible = false;
  }

  onDelete() {
    if (this.currentReview) {
      const review: IAddReview = {
        revieweeId: this.userId,
        rating: this.currentReview.rating,
        comment: this.currentReview.comment,
        isAnonymous: this.currentReview.isAnonymous
      };
      this.confirm.emit({review, mode: 'delete'});
      this.visible = false;
    }
  }

  async deleteReview() {
    const confirmed = await this.confirmationDialogService.confirm(
      'Delete Review?',
      'You will be able to review this influencer again.',
      'Delete',
      'Cancel'
    );
    if (confirmed) {
      this.onDelete();
    } else {
      console.log('Deletion canceled');
    }
  }
}
