import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { IRequestInfluencer } from '../../models/request.model';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { PlatformIconPipe } from '../../pipes/platform-icon.pipe';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper } from '@ng-icons/bootstrap-icons';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';
import { PlatformService } from '../../services/platform.service';
import { MediaIconPipe } from '../../pipes/media-icon.pipe';
import { ConfirmationDialogService } from '../confirmation-dialog/confirmation-dialog.service';

@Component({
  selector: 'app-request-pending',
  standalone: true,
  imports: [DatePipe, PlatformIconPipe, NgIconComponent, MediaIconPipe],
  providers: [provideIcons({ simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper })],
  templateUrl: './request-pending.component.html',
  styleUrl: './request-pending.component.css'
})
export class RequestPendingComponent {

  @Input() 
  request!: IRequestInfluencer;

  @Output()
  cancelRequestEvent = new EventEmitter<number>();

  router: Router = inject(Router);

  platformService = inject(PlatformService);
  confirmationDialogService = inject(ConfirmationDialogService);

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }

  async cancelRequest(requestId: number) {
    const confirmed = await this.confirmationDialogService.confirm(
      'Withdraw Request?',
      'You will be able to request key to this campaign later.',
      'Withdraw',
      'Cancel'
    );
    if (confirmed) {
      this.cancelRequestEvent.emit(requestId);
    } else {
      console.log('Deletion canceled');
    }
  }

}
