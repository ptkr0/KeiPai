import { Component, inject } from '@angular/core';
import { UserService } from '../../services/user.service';
import { DeveloperCampaignsComponent } from "../developer-campaigns/developer-campaigns.component";
import { InfluencerCampaignsComponent } from "../influencer-campaigns/influencer-campaigns.component";

@Component({
  selector: 'app-campaigns',
  standalone: true,
  imports: [DeveloperCampaignsComponent, InfluencerCampaignsComponent],
  templateUrl: './campaigns.component.html',
  styleUrl: './campaigns.component.css'
})
export class CampaignsComponent {
  userService = inject(UserService);
}
