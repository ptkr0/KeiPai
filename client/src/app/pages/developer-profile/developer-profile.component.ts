import { Component, inject, Input, OnInit } from '@angular/core';
import { IDeveloperWithInfo } from '../../models/developer.model';
import { bootstrapEnvelopeFill, bootstrapPersonFill, bootstrapExclamationCircle, bootstrapGearFill, bootstrapGraphUp, bootstrapListCheck } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { CampaignService } from '../../services/campaign.service';
import { GameService } from '../../services/game.service';
import { IGame } from '../../models/game.model';
import { GameComponent } from "../../components/game/game.component";
import { CampaignComponent } from "../../components/campaign/campaign.component";
import { ICampaign } from '../../models/campaign.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-developer-profile',
  standalone: true,
  imports: [CommonModule, NgIconComponent, GameComponent, CampaignComponent],
  viewProviders: [provideIcons({ bootstrapEnvelopeFill, bootstrapPersonFill, bootstrapExclamationCircle, bootstrapGearFill, bootstrapGraphUp, bootstrapListCheck })],
  templateUrl: './developer-profile.component.html',
  styleUrl: './developer-profile.component.css'
})
export class DeveloperProfileComponent implements OnInit {

  @Input()
  profile: IDeveloperWithInfo | undefined = undefined;

  userService = inject(UserService);
  gameService = inject(GameService);
  campaignService = inject(CampaignService);
  activatedRoute = inject(ActivatedRoute);
  router = inject(Router);

  selectedTab: string = 'games';
  games: IGame[] = [];
  campaigns: ICampaign[] = [];

  ngOnInit() {
    const tab = this.activatedRoute.snapshot.queryParamMap.get('tab');
    if (tab) {
      this.selectTab(tab);
    } else {
      this.selectTab('games');
    }
  }
  

  fetchGames() {
    if (this.games.length > 0) {
      console.log('Games already fetched:', this.games);
      return;
    }
    
      if (this.profile && this.profile.developer) {
        this.gameService.getGames(this.profile.developer.id, 1).subscribe({
        next: (res) => {
          this.games = res.games;
        },
        error: (error) => {
          console.error('Error fetching games:', error);
        },
      })
    }
  }

  fetchCampaigns() {
    if (this.campaigns.length > 0) {
      console.log('Campaigns already fetched:', this.campaigns);
      return;
    }
    if (this.profile && this.profile.developer) {
      this.campaignService.getCampaignsForInfluencer(1, this.profile.developer.id, undefined, undefined, true).subscribe({
        next: (res) => {
          this.campaigns = res.campaigns;
          console.log('Campaigns:', res.campaigns);
        },
        error: (error) => {
          console.error('Error fetching campaigns:', error);
        },
      })
    }
  }

  selectTab(tab: string) {
    this.selectedTab = tab;
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { tab },
      queryParamsHandling: 'merge',
    });
  
    if (tab === 'games') {
      this.fetchGames();
    } else if (tab === 'campaigns') {
      this.fetchCampaigns();
    }
  }
  
}
