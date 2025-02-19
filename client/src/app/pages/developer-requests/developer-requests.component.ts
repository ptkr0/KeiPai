import { Component, inject } from '@angular/core';
import { DevNavigationComponent } from "../../components/dev-navigation/dev-navigation.component";
import { CampaignService } from '../../services/campaign.service';
import { RequestService } from '../../services/request.service';
import { IActiveCampaign } from '../../models/campaign.model';
import { IRequestDeveloper } from '../../models/request.model';
import { DatePipe } from '@angular/common';
import { bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';
import { MediaIconPipe } from '../../pipes/media-icon.pipe';
import { PlatformIconPipe } from '../../pipes/platform-icon.pipe';
import { Router } from '@angular/router';
import { CountryPipe } from "../../pipes/country.pipe";
import { RatingComponent } from "../../components/rating/rating.component";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-developer-requests',
  standalone: true,
  imports: [DatePipe, PlatformIconPipe, NgIconComponent, MediaIconPipe, DevNavigationComponent, CountryPipe, RatingComponent],
  providers: [provideIcons({ simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle })],
  templateUrl: './developer-requests.component.html',
  styleUrl: './developer-requests.component.css'
})
export class DeveloperRequestsComponent {

  campaignService = inject(CampaignService);
  requestService = inject(RequestService);
  toast = inject(ToastrService);

  router = inject(Router);

  activeCampaigns: IActiveCampaign[] = [];
  requests: IRequestDeveloper[] = [];

  selectedCampaignIds: number[] = [];
  
  toggleCampaignSelection(campaignId: number) {
    const index = this.selectedCampaignIds.indexOf(campaignId);
    if (index === -1) {
      this.selectedCampaignIds.push(campaignId);
    } else {
      this.selectedCampaignIds.splice(index, 1);
    }
  }

  filterRequests() {
    this.fetchRequests(this.selectedCampaignIds.length ? this.selectedCampaignIds : undefined);
  }

  constructor() {
    this.fetchActiveCampaigns();
    this.fetchRequests();
  }

  fetchActiveCampaigns() {
    this.campaignService.getActiveCampaignsForDev().subscribe({
      next: (res) => {
        this.activeCampaigns = res;
      },
      error: (error) => {
        this.toast.error('Failed to fetch active campaigns');
      }
    });
  }

  fetchRequests(campaignId?: number[]) {
    this.requestService.getRequestsForDeveloper(campaignId, "pending").subscribe({
      next: (res) => {
        this.requests = res;
      },
      error: (error) => {
        this.toast.error('Failed to fetch requests');
      }
    });
  }

  canAccept(request: IRequestDeveloper): boolean {
    const campaign = this.activeCampaigns.find(c => c.id === request.campaignId);
    if (!campaign) {
      return false;
    }
  
    const platformKeys = campaign.keys.find(k => k.id === request.platform);
    if (!platformKeys) {
      return false;
    }
  
    if (platformKeys.keysForCampaign === -1) {
      return platformKeys.keysLeft > 0;
    }
  
    return platformKeys.keysForCampaign > platformKeys.acceptedRequests && platformKeys.keysLeft > 0;
  }

  getKeys(campaignId: number): string {
    const campaign = this.activeCampaigns.find(c => c.id === campaignId);
    if (!campaign) {
      return '0/0';
    }
    return campaign.keys.map(key => {
      const keysAssigned = key.keysForCampaign === -1 ? 'inf' : key.keysForCampaign;
      return `${key.name}: ${key.acceptedRequests}/${keysAssigned} (${key.keysLeft})`;
    }).join(', ');
  }

  acceptRequest(request: IRequestDeveloper) {
    this.requestService.handleRequest(request.id, 1).subscribe({
      next: () => {
        const campaign = this.activeCampaigns.find(c => c.id === request.campaignId);
        if (!campaign) {
          return;
        }
        campaign.keys = campaign.keys.map(key => {
          if (key.id === request.platform) {
            key.keysLeft--;
            key.acceptedRequests++;
          }
          return key;
        }, []);
        this.requests = this.requests.filter(r => r.id !== request.id);

        this.toast.success('Request accepted');
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error accepting request';
        this.toast.error(errorMessage);
      }
    });
  }

  rejectRequest(request: IRequestDeveloper) {
    this.requestService.handleRequest(request.id, 2).subscribe({
      next: () => {
        this.requests = this.requests.filter(r => r.id !== request.id);
        this.toast.success('Request rejected');
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error accepting request';
        this.toast.error(errorMessage);
      }
    });
  }

  redirector(id: string, path: string) {
    this.router.navigate([path, id]);
  }
}
