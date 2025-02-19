import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { ICampaign } from '../../models/campaign.model';
import { CampaignService } from '../../services/campaign.service';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { CampaignComponent } from "../../components/campaign/campaign.component";
import { InfluencerNavigationComponent } from "../../components/influencer-navigation/influencer-navigation.component";
import { PlatformService } from '../../services/platform.service';
import { IPlatform } from '../../models/platform.model';
import { ITag } from '../../models/tag.model';
import { TagService } from '../../services/tag.service';
import { IPagination } from '../../models/pagination.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-influencer-campaigns',
  standalone: true,
  imports: [CampaignComponent, InfluencerNavigationComponent, CommonModule, FormsModule,],
  templateUrl: './influencer-campaigns.component.html',
  styleUrl: './influencer-campaigns.component.css'
})
export class InfluencerCampaignsComponent {

  userService = inject(UserService);
  campaignService = inject(CampaignService);
  platformService = inject(PlatformService);
  tagService = inject(TagService);
  toast = inject(ToastrService);

  http: HttpClient = inject(HttpClient);
  router: Router = inject(Router);

  campaigns: ICampaign[] = [];
  pagination: IPagination | null = null;
  platforms: Array<IPlatform & { selected: boolean }> = [];
  tags: Array<ITag & { selected: boolean }> = [];
  
  filteredTags: Array<ITag & { selected: boolean }> = [];
  selectedTags: string[] = [];
  selectedPlatforms: string[] = [];
  isIncludeSoonSelected: boolean = false;

  constructor() {
    this.fetchCampaigns();
    this.fetchTags();
    this.platforms = this.platformService.getPlatforms().map(platform => ({
      ...platform,
      selected: false
    }));
  }

  fetchCampaigns(pageNumber: number = 1, platforms: string[] = [], tags: string[] = [], includeComingSoon: boolean = false) {
    this.campaignService.getCampaignsForInfluencer(pageNumber, undefined, platforms, tags, includeComingSoon).subscribe({
      next: (res) => {
        this.campaigns = res.campaigns;
        this.pagination = res;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching campaigns';
        this.toast.error(errorMessage);
      }
    });
  }

  fetchTags() {
    this.tagService.getTags().subscribe({
      next: (res) => {
        this.tags = res.map(tag => ({
          ...tag,
          selected: false
        }));
        this.filteredTags = this.tags;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching tags';
        this.toast.error(errorMessage);
      }
    });
  }

  onFilterInputChange(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const filterValue = inputElement.value.toLowerCase();
    this.filteredTags = this.tags.filter(tag => tag.name.toLowerCase().includes(filterValue));
  }

  onFilterClick() {
    this.selectedPlatforms = this.platforms
      .filter(platform => platform.selected)
      .map(platform => platform.id.toString());
    this.selectedTags = this.tags
      .filter(tag => tag.selected)
      .map(tag => tag.id.toString());
    this.fetchCampaigns(1, this.selectedPlatforms, this.selectedTags, this.isIncludeSoonSelected);
  }

  prevPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage > 1) {
      this.fetchCampaigns(this.pagination.currentPage - 1, this.selectedPlatforms, this.selectedTags, this.isIncludeSoonSelected);
    }
  }

  nextPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage < this.pagination.totalPages) {
      this.fetchCampaigns(this.pagination.currentPage + 1, this.selectedPlatforms, this.selectedTags, this.isIncludeSoonSelected);
    }
  }
}
