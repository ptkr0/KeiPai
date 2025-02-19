import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { IRequestInfluencer } from '../../models/request.model';
import { PlatformService } from '../../services/platform.service';
import { ConfirmationDialogService } from '../confirmation-dialog/confirmation-dialog.service';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';
import { MediaIconPipe } from '../../pipes/media-icon.pipe';
import { PlatformIconPipe } from '../../pipes/platform-icon.pipe';
import { IBasicGame } from '../../models/game.model';

@Component({
  selector: 'app-request-accepted',
  standalone: true,
  imports: [DatePipe, PlatformIconPipe, NgIconComponent, MediaIconPipe],
  providers: [provideIcons({ simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle })],
  templateUrl: './request-accepted.component.html',
  styleUrl: './request-accepted.component.css'
})
export class RequestAcceptedComponent {
  @Input() 
  request!: IRequestInfluencer;

  @Output()
  cancelRequestEvent = new EventEmitter<number>();

  @Output()
  onOpenDialog = new EventEmitter<{game: IBasicGame, requestId: number}>();

  router: Router = inject(Router);

  platformService = inject(PlatformService);
  confirmationDialogService = inject(ConfirmationDialogService);

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }

  copyToClipboard(toCopy: string) : void {
    const create_copy = (e : ClipboardEvent) => {
        e.clipboardData!.setData('text/plain', toCopy);
        e.preventDefault();
    };
    document.addEventListener('copy', create_copy );
    document.execCommand('copy');
    document.removeEventListener('copy', create_copy );
  }

  openSubmit() {
    if (this.request.media === 'youtube') {
      const submit_youtube = document.getElementById('submit_youtube') as HTMLDialogElement;
      if (submit_youtube) {
        submit_youtube.showModal();
      }
    }
    else if(this.request.media === 'other')
    {
      let game: IBasicGame = { id: this.request.gameId, name: this.request.gameName, cover: this.request.gameCover };
      this.onOpenDialog.emit({game: game, requestId: this.request.id});
    }
  }
}

