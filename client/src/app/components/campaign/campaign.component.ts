import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ICampaign } from '../../models/campaign.model';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from '../confirmation-dialog/confirmation-dialog.service';
import { UserService } from '../../services/user.service';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { PlatformIconPipe } from '../../pipes/platform-icon.pipe';
import { simpleBattledotnet, simpleEpicgames, simpleGogdotcom, simpleItchdotio, simpleOrigin, simpleSteam, simpleUbisoft } from '@ng-icons/simple-icons';
import { bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe } from '@ng-icons/bootstrap-icons';
import { DatePipe } from '@angular/common';
import { CampaignService } from '../../services/campaign.service';

@Component({
  selector: 'app-campaign',
  standalone: true,
  imports: [NgIconComponent, PlatformIconPipe, DatePipe],
  providers: [provideIcons({ simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe })],
  templateUrl: './campaign.component.html',
  styleUrl: './campaign.component.css'
})
export class CampaignComponent {

  router = inject(Router);
  @Input()
  campaign?: ICampaign;
  confirmationDialogService = inject(ConfirmationDialogService);
  userService = inject(UserService);
  campaignService = inject(CampaignService);

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }

  hasEnded(): boolean {
    if (this.campaign?.isClosed) {
      return true;
    }
    if (this.campaign?.endDate) {
      return new Date(this.campaign.endDate) < new Date();
    }
    return false;
  }

  async closeCampaign() {
    const confirmed = await this.confirmationDialogService.confirm(
      'Close Campaign for '+ this.campaign?.game?.name+'?',
      'All incoming requests will be rejected.',
      'Close',
      'Cancel'
    );
    if (confirmed) {
        this.campaignService.closeCampaign(this.campaign!.id).subscribe(
          () => {
            this.campaign!.isClosed = true;
          },
          (error) => {
            console.error('Error closing campaign:', error);
          }
        );
      } else {
        console.log('Close canceled');
      }
  }
}
  