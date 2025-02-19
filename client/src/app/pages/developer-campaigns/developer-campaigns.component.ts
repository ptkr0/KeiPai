import { Component, inject } from '@angular/core';
import { GameComponent } from "../../components/game/game.component";
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { DevNavigationComponent } from "../../components/dev-navigation/dev-navigation.component";
import { CampaignService } from '../../services/campaign.service';
import { ICampaign } from '../../models/campaign.model';
import { CampaignComponent } from "../../components/campaign/campaign.component";
import { IPagination } from '../../models/pagination.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-developer-campaigns',
  standalone: true,
  imports: [DevNavigationComponent, CampaignComponent],
  templateUrl: './developer-campaigns.component.html',
  styleUrl: './developer-campaigns.component.css'
})
export class DeveloperCampaignsComponent {

  userService = inject(UserService);
  campaignService = inject(CampaignService);
  toast = inject(ToastrService);

  http: HttpClient = inject(HttpClient);
  router: Router = inject(Router);

  campaigns: ICampaign[] = [];
  pagination: IPagination | null = null;

  constructor() {
    this.fetchCampaigns();
  }

  fetchCampaigns(pageNumber: number = 1) {
    if (this.campaigns.length > 0) {
      return;
    }
    this.campaignService.getCampaignsForDev(pageNumber).subscribe({
      next: (res) => {
        this.campaigns = res.campaigns;
        this.pagination = res;
      },
      error: (error) => {
        this.toast.error('Failed to fetch campaigns');
      }
    });
  }

  redirectToAddCampaign() {
    this.router.navigateByUrl('/add-campaign');
  }
}