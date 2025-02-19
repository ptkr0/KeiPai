import { Component, inject, Input, OnInit } from '@angular/core';
import { IDeveloperWithInfo } from '../../models/developer.model';
import { CampaignService } from '../../services/campaign.service';
import { GameService } from '../../services/game.service';
import { UserService } from '../../services/user.service';
import { IInfluencerWithInfo } from '../../models/influencer.model';
import { CommonModule } from '@angular/common';
import { bootstrapEnvelopeFill, bootstrapPersonFill, bootstrapExclamationCircle, bootstrapGearFill, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapChatSquareTextFill } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { CountryPipe } from '../../pipes/country.pipe';
import { VideoListComponent } from "../../components/video-list/video-list.component";
import { OtherMediaListComponent } from "../../components/other-media-list/other-media-list.component";
import { ActivatedRoute, Router } from '@angular/router';
import { RatingComponent } from "../../components/rating/rating.component";
import { ReviewListComponent } from "../../components/review-list/review-list.component";
import { StreamListComponent } from "../../components/stream-list/stream-list.component";

@Component({
  selector: 'app-influencer-profile',
  standalone: true,
  imports: [CommonModule, NgIconComponent, CountryPipe, VideoListComponent, OtherMediaListComponent, RatingComponent, ReviewListComponent, StreamListComponent],
  viewProviders: [provideIcons({ bootstrapEnvelopeFill, bootstrapPersonFill, bootstrapExclamationCircle, bootstrapGearFill, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapChatSquareTextFill })],
  templateUrl: './influencer-profile.component.html',
  styleUrl: './influencer-profile.component.css'
})
export class InfluencerProfileComponent implements OnInit {

  @Input()
  profile: IInfluencerWithInfo | undefined = undefined;

  selectedTab: string = 'none';

  userService = inject(UserService);
  gameService = inject(GameService);
  campaignService = inject(CampaignService);
  activatedRoute = inject(ActivatedRoute);
  router = inject(Router);

  ngOnInit() {
    const tab = this.activatedRoute.snapshot.queryParamMap.get('tab');
    if (tab) {
      this.selectedTab = tab;
    } else if (this.profile) {
      if (this.profile.media.youtube) this.selectedTab = 'youtube';
      else if (this.profile.media.twitch) this.selectedTab = 'twitch';
      else if (this.profile.media.otherMedia) this.selectedTab = 'other';
      else this.selectedTab = 'none';
    }
  }

  selectTab(tab: string) {
    this.selectedTab = tab;
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { tab },
      queryParamsHandling: 'merge',
    });
  }
}
