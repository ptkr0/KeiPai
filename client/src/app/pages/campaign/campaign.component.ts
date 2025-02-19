import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationDialogService } from '../../components/confirmation-dialog/confirmation-dialog.service';
import { GameService } from '../../services/game.service';
import { UserService } from '../../services/user.service';
import { bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapEnvelopeFill, bootstrapPersonFill, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapNintendoSwitch, bootstrapPlaystation, bootstrapXbox, bootstrapKeyFill } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { MarkdownComponent, provideMarkdown } from 'ngx-markdown';
import { CommonModule } from '@angular/common';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { CampaignService } from '../../services/campaign.service';
import { ICampaignDetail, ICanRequest } from '../../models/campaign.model';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';
import { PlatformIconPipe } from "../../pipes/platform-icon.pipe";
import { RequestKeyDialogComponent } from "../../components/request-key-dialog/request-key-dialog.component";
import { InfluencerService } from '../../services/influencer.service';
import { IInfluencerInfo } from '../../models/influencer.model';
import { ISendRequest } from '../../models/request.model';
import { RequestService } from '../../services/request.service';
import { ToastrService } from 'ngx-toastr';

interface MediaItem {
  type: 'image' | 'video';
  source: string;
}

export interface FinalPlatforms {
  youtube: boolean;
  twitch: boolean;
  other: boolean;
}

@Component({
  selector: 'app-campaign',
  standalone: true,
  imports: [CommonModule, MarkdownComponent, NgIconComponent, YouTubePlayerModule, PlatformIconPipe, RequestKeyDialogComponent],
  providers: [provideMarkdown()],
  viewProviders: [provideIcons({ bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapEnvelopeFill, bootstrapPersonFill,simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapKeyFill })],
  templateUrl: './campaign.component.html',
  styleUrl: './campaign.component.css'
})
export class CampaignComponent {
  
  activatedRoute = inject(ActivatedRoute);
  router = inject(Router);
  id: number;

  gameService = inject(GameService);
  userService = inject(UserService);
  campaignService = inject(CampaignService);
  influencerService = inject(InfluencerService);
  requestService = inject(RequestService);
  toast = inject(ToastrService);

  campaign: ICampaignDetail | null = null;
  videoId: string | null = null;
  canRequest: ICanRequest | null = null;
  influencerStats: IInfluencerInfo | null = null;
  selectedMedia: MediaItem | null = null;
  confirmationDialogService = inject(ConfirmationDialogService);

  finalPlatforms: FinalPlatforms = {
    youtube: false,
    twitch: false,
    other: false
  };

  isRequestKeyDialogVisible = false;

  constructor() {
    this.id = this.activatedRoute.snapshot.params['id'];
    this.fetchCampaign();
  }

  getVideoCoverUrl(id: string | null): string {
    if (id) {
      return `https://img.youtube.com/vi/${id}/maxresdefault.jpg`;
    }
    return '';
  }

  extractVideoId(url: string): string | null {
    const regex = /(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
    const match = url.match(regex);
    return match ? match[1] : null;
  }

  onMediaClick(media: MediaItem) {
    this.selectedMedia = media;
  }

  onRequestClick() {
    this.isRequestKeyDialogVisible = true;
  }

  onRequestDialogCancel() {
    this.isRequestKeyDialogVisible = false;
  }

  onRequestDialogConfirm(request: ISendRequest) {
    this.isRequestKeyDialogVisible = false;
    this.requestService.sendRequest(request).subscribe({
      next: (response) => {
        this.toast.success('Request sent successfully');
        this.canRequest = { canRequest: false, reasonCode: 0, reasonMessage: "User has already sent request to this campaign" };
      },
      error: (error) => {
        this.toast.error('Error sending request');
      }
    });
  }

  fetchCampaign() {
    this.campaignService.getCampaign(this.id).subscribe({
      next: (campaign) => {
        this.campaign = campaign;
        if (this.campaign.game.youtubeTrailer) {
          this.videoId = this.extractVideoId(this.campaign.game.youtubeTrailer);
          this.selectedMedia = { type: 'video', source: this.videoId ?? '' };
        } else if (this.campaign.game.screenshots && this.campaign.game.screenshots.length > 0) {
          this.selectedMedia = { type: 'image', source: this.campaign.game.screenshots[0].screenshot };
        }
        if(this.userService.user()?.role === 'Influencer')
          {
            this.campaignService.checkIfCanJoin(this.id).subscribe({
              next: (canRequest) => {
                this.canRequest = canRequest;
                if(canRequest.canRequest === true){
                this.influencerService.getInfluencerInfo().subscribe({
                  next: (influencerStats) => {
                    this.influencerStats = influencerStats;
                    this.checkApplicablePlatforms(influencerStats);
                  },
                  error: (error) => {
                    this.toast.error('Error fetching influencer info');
                  }
                });
              }
              },
              error: (error) => {
                this.toast.error('Error fetching campaign');
              }
            });
          }
      },
      error: (error) => {
        this.toast.error('Error fetching campaign');
        this.router.navigate(['/campaigns']);
      }
    });
  }

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }

  hasEnded(): boolean {
    if (this.campaign?.endDate) {
      return new Date(this.campaign.endDate) < new Date();
    }
    if (this.campaign?.isClosed) {
      return true;
    }
    return false;
  }

  hasStarted(): boolean {
    if (this.campaign?.startDate){
      let start: Date = new Date(this.campaign?.startDate)
      if (start > new Date()) {
        return false
      }
      else {
        return true
      }
    }
    return false
  }

  checkIfCanRequest() {
    if (this.canRequest?.canRequest === true) {
      return true;
    }
    return false;
  }

  checkFinalIfCanRequest() {
    if ((this.finalPlatforms.youtube || this.finalPlatforms.twitch || this.finalPlatforms.other) && this.canRequest?.canRequest === true) {
      return true;
    }
    return false;
  }

  checkApplicablePlatforms(stats: IInfluencerInfo): void {
    // influencer has no youtube account
    if (!stats.youtube) {
      this.finalPlatforms.youtube = false;
    } else {
      this.finalPlatforms.youtube = false; // Default to false
      if (this.campaign?.minimumYoutubeSubscribers !== null) {
        if (this.campaign!.minimumYoutubeSubscribers <= stats.youtube.subscriberCount) {
          this.finalPlatforms.youtube = true;
        }
      }
      if (this.campaign?.minimumYoutubeAvgViews !== null) {
        if (this.campaign!.minimumYoutubeAvgViews <= stats.youtube.averageViewCount) {
          this.finalPlatforms.youtube = true;
        }
      }
    }
  
    // influencer has no twitch account
    if (!stats.twitch) {
      this.finalPlatforms.twitch = false;
    } else {
      this.finalPlatforms.twitch = false; // Default to false
      if (this.campaign?.minimumTwitchFollowers !== null) {
        if (this.campaign!.minimumTwitchFollowers <= stats.twitch.followerCount) {
          this.finalPlatforms.twitch = true;
        }
      }
      if (this.campaign?.minimumTwitchAvgViewers !== null) {
        if (this.campaign!.minimumTwitchAvgViewers <= stats.twitch.averageViewers) {
          this.finalPlatforms.twitch = true;
        }
      }
    }
  
    // influencer has no third party account
    if (!stats.otherMedia) {
      this.finalPlatforms.other = false;
    } else {
      if (this.campaign?.areThirdPartyWebsitesAllowed === 1) {
        this.finalPlatforms.other = true;
      } else if (this.campaign?.areThirdPartyWebsitesAllowed === 2) {
        if (stats.otherMedia.isVerified) {
          this.finalPlatforms.other = true;
        }
      } else {
        this.finalPlatforms.other = false;
      }
    }

    if (!this.finalPlatforms.youtube && !this.finalPlatforms.twitch && !this.finalPlatforms.other && this.canRequest?.reasonMessage === '') {
      this.canRequest = { canRequest: false, reasonCode: 3, reasonMessage: "User doesn't meet any of the requirements" };
    }
  }

}
