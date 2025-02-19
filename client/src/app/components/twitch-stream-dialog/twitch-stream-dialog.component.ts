import { Component, inject } from '@angular/core';
import { IGetTwitchStream } from '../../models/twitch.model';
import { TwitchService } from '../../services/twitch.service';
import { TwitchStreamDialogService } from './twitch-stream-dialog.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-twitch-stream-dialog',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './twitch-stream-dialog.component.html',
  styleUrl: './twitch-stream-dialog.component.css'
})
export class TwitchStreamDialogComponent {
  visible = false;
  loading = true;
  streamId: number | undefined = undefined;
  stream: IGetTwitchStream | undefined = undefined;

  twitchService = inject(TwitchService);

  constructor(private twitchStreamDialogService: TwitchStreamDialogService) {
    this.twitchStreamDialogService.dialogData$.subscribe(data => {
      if (data) {
        this.streamId = data.streamId;
        this.visible = true;
        this.fetchContent();
      } else {
        this.visible = false;
      }
    });
  }

  fetchContent() {
    this.loading = true;
    if (this.streamId) {
      this.twitchService.getStream(this.streamId).subscribe({
        next: (res) => {
          this.loading = false;
          this.stream = res;
          console.log('Contents:', res);
        },
        error: (error) => {
          this.loading = false;
          console.error('Error fetching contents:', error);
        }
      });
    }
  }

  getStreamDuration() {
    if (!this.stream) {
      return '00:00:00';
    }
    const start = new Date(this.stream.stream.startDate);
    const end = new Date(this.stream.stream.endDate);

    const duration = end.getTime() - start.getTime();
    const hours = Math.floor(duration / (1000 * 60 * 60));
    const minutes = Math.floor((duration % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((duration % (1000 * 60)) / 1000);
    
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  }

  getProperThumbnailUrl() {
    // https://static-cdn.jtvnw.net/cf_vods/dgeft87wbj63p/0c5e2cc33313ac2b750a_ptkr_314941790713_1738165743//thumb/thumb0-%{width}x%{height}.jpg
    if (!this.stream) {
      return '';
    }
    return this.stream.stream.thumbnail.replace('%{width}', '250').replace('%{height}', '150');
  }

  onConfirm() {
    this.visible = false;
  }
}
