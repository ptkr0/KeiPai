import { Component, inject } from '@angular/core';
import { UserService } from '../../services/user.service';
import { CampaignService } from '../../services/campaign.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ICampaignStats } from '../../models/campaign.model';
import { DatePipe } from '@angular/common';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { PlatformIconPipe } from '../../pipes/platform-icon.pipe';
import { bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapEnvelopeFill, bootstrapPersonFill, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapKeyFill, bootstrapExclamationCircle } from '@ng-icons/bootstrap-icons';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';
import { IRequestDeveloper } from '../../models/request.model';
import { RequestService } from '../../services/request.service';
import { MediaIconPipe } from "../../pipes/media-icon.pipe";
import { CountryPipe } from "../../pipes/country.pipe";
import { OtherMediaDialogService } from '../../components/other-media-dialog/other-media-dialog.service';
import { YoutubeVideoDialogService } from '../../components/youtube-video-dialog/youtube-video-dialog.service';
import { RatingComponent } from "../../components/rating/rating.component";
import { AddReviewComponent, AddReviewDialogResponse } from "../../components/add-review/add-review.component";
import { IAddReview } from '../../models/review.model';
import { ReviewService } from '../../services/review.service';
import e from 'express';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-campaign-details',
  standalone: true,
  imports: [DatePipe, NgIconComponent, PlatformIconPipe, MediaIconPipe, CountryPipe, RatingComponent, AddReviewComponent],
  providers: [provideIcons({ simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle })],
  templateUrl: './campaign-details.component.html',
  styleUrl: './campaign-details.component.css'
})
export class CampaignDetailsComponent {

  activatedRoute = inject(ActivatedRoute);
  userService = inject(UserService);
  router = inject(Router);

  campaignService = inject(CampaignService);
  requestService = inject(RequestService);
  reviewService = inject(ReviewService);
  otherMediaDialogService = inject(OtherMediaDialogService);
  youtubeVideoDialogService = inject(YoutubeVideoDialogService);
  toast = inject(ToastrService);

  campaignStats: ICampaignStats | undefined = undefined;
  requests: IRequestDeveloper[] = [];
  id: number;

  selectedTab: string = 'accepted-and-completed';

  isAddRequestVisible: boolean = false;
  selectedRequestUserId: string = '';
  wasUserRated: boolean = false;

  constructor() {
      this.id = this.activatedRoute.snapshot.params['id'];
      const tab = this.activatedRoute.snapshot.queryParamMap.get('tab');
      if (tab) {
        this.selectedTab = tab;
      }
      this.fetchStats();
      this.fetchRequests(tab ?? tab ?? 'accepted-and-completed');
   };

  fetchStats() {
    this.campaignService.getCampaignStats(this.id).subscribe({
      next: (res) => {
        this.campaignStats = res;
        console.log('Campaign:', res);
      },
      error: (error) => {
        console.error('Error fetching campaign:', error);
      }
    });
  }

  fetchRequests(option: string = "accepted-and-completed") {
    this.requestService.getRequestsForDeveloper([this.id], option).subscribe({
      next: (res) => {
        this.requests = res;
        console.log('Requests:', res);
      },
      error: (error) => {
        console.error('Error fetching requests:', error);
      }
    });
  }

  selectTab(tab: string) {
    this.selectedTab = tab;
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { tab },
      queryParamsHandling: 'merge',
    });
  }

  onFilterChange(option: string) {
    this.selectTab(option);
    this.fetchRequests(option);
  }

  redirector(id: any, path: string) {
    this.router.navigate([path, id]);
  }

  hasEnded(): boolean {
    if (this.campaignStats?.campaign?.isClosed) {
      return true;
    }
    if (this.campaignStats?.campaign?.endDate) {
      return new Date(this.campaignStats?.campaign.endDate) < new Date();
    }
    return false;
  }

  openMediaView(request: IRequestDeveloper) {
    if (typeof request.content === 'number') {
      if (request.media === 'other') {
        this.otherMediaDialogService.open(request.content);
      } else if (request.media === 'youtube') {
        this.youtubeVideoDialogService.open(request.content);
      }
    } else {
      console.error('Invalid content type:', request.content);
    }
  }

  getStatus(request: IRequestDeveloper): string {
    if(request.status === 3) {
      return 'Completed';
    }
    else if(request.status === 1 && request.content) {
      return 'Completed';
    }
    else {
      return 'Pending'
    }
  }

  onAddReview(userId: string, userWasRated: boolean) {
    this.selectedRequestUserId = userId;
    this.wasUserRated = userWasRated;
    this.isAddRequestVisible = true;
  }

  cancelAddReview() {
    this.isAddRequestVisible = false;
  }

  addReview(response: AddReviewDialogResponse) {
    this.isAddRequestVisible = false;

    if(response.mode === 'delete') {
      this.reviewService.deleteReview(response.review.revieweeId).subscribe({
        next: () => {
            const request = this.requests.find(r => r.influencerId === response.review.revieweeId);
            if (!request) {
            return;
            }
            
            request.influencerRating = {
            numberOfRatings: request.influencerRating.numberOfRatings - 1,
            totalRating: request.influencerRating.totalRating - response.review.rating,
            userWasRated: false
            };

            this.toast.success('Review deleted successfully');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error deleting review';
          this.toast.error(errorMessage);
        }
      });
    }
    else if(response.mode === 'add') {
      this.reviewService.addReview(response.review).subscribe({
        next: (review) => {
          const request = this.requests.find(r => r.influencerId === response.review.revieweeId);
          if (!request) {
            return;
          }

          request.influencerRating = {
            numberOfRatings: request.influencerRating.numberOfRatings + 1,
            totalRating: request.influencerRating.totalRating + review.rating,
            userWasRated: true
          };

          this.toast.success('Review added successfully');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error adding review';
          this.toast.error(errorMessage);
        }
      });
    }

    else if(response.mode === 'edit') {
      this.reviewService.updateReview(response.review).subscribe({
        next: (review) => {
          console.log(review);
          const request = this.requests.find(r => r.influencerId === response.review.revieweeId);
          if (!request) {
            return;
          }

          request.influencerRating = {
            numberOfRatings: request.influencerRating.numberOfRatings,
            totalRating: request.influencerRating.totalRating + review.updatedReview.rating - review.oldRating,
            userWasRated: true
          };

          this.toast.success('Review edited successfully');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error editing review';
          this.toast.error(errorMessage);
        }
      });
    }
  }
}
