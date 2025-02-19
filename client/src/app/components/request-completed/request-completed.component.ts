import { Component, inject, Input } from '@angular/core';
import { IRequestInfluencer } from '../../models/request.model';
import { PlatformService } from '../../services/platform.service';
import { Router } from '@angular/router';
import { bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';
import { MediaIconPipe } from '../../pipes/media-icon.pipe';
import { PlatformIconPipe } from '../../pipes/platform-icon.pipe';
import { OtherMediaDialogService } from '../other-media-dialog/other-media-dialog.service';
import { YoutubeVideoDialogService } from '../youtube-video-dialog/youtube-video-dialog.service';
import { TwitchStreamDialogService } from '../twitch-stream-dialog/twitch-stream-dialog.service';

@Component({
  selector: 'app-request-completed',
  standalone: true,
  imports: [PlatformIconPipe, NgIconComponent, MediaIconPipe],
  providers: [provideIcons({ simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio, bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe, bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapExclamationCircle })],
  templateUrl: './request-completed.component.html',
  styleUrl: './request-completed.component.css'
})
export class RequestCompletedComponent {

  @Input() 
  request!: IRequestInfluencer;

  router: Router = inject(Router);

  platformService = inject(PlatformService);
  otherMediaDialogService = inject(OtherMediaDialogService);
  youtubeVideoDialogService = inject(YoutubeVideoDialogService);
  twitchStreamDialogService = inject(TwitchStreamDialogService);

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }

  openMediaView() {
    if (typeof this.request.content === 'number') {
      if (this.request.media === 'other') {
        this.otherMediaDialogService.open(this.request.content);
      } else if (this.request.media === 'youtube') {
        this.youtubeVideoDialogService.open(this.request.content);
      } else if (this.request.media === 'twitch') {
        this.twitchStreamDialogService.open(this.request.content);
      }
    } else {
      console.error('Invalid content type:', this.request.content);
    }
  }
}