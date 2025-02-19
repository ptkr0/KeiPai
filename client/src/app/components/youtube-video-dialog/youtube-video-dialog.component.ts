import { Component, inject } from '@angular/core';
import { YoutubeVideoDialogService } from './youtube-video-dialog.service';
import { YoutubeService } from '../../services/youtube.service';
import { IGetYoutubeVideo } from '../../models/youtube.model';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-youtube-video-dialog',
  standalone: true,
  imports: [YouTubePlayerModule, DatePipe],
  templateUrl: './youtube-video-dialog.component.html',
  styleUrl: './youtube-video-dialog.component.css'
})
export class YoutubeVideoDialogComponent {
  visible = false;
  loading = true;
  videoId: number | undefined = undefined;
  video: IGetYoutubeVideo | undefined = undefined;

  youtubeService = inject(YoutubeService);

  constructor(private youtubeVideoDialogService: YoutubeVideoDialogService) {
    this.youtubeVideoDialogService.dialogData$.subscribe(data => {
      if (data) {
        this.videoId = data.videoId;
        this.visible = true;
        this.fetchContent();
      } else {
        this.visible = false;
      }
    });
  }

  fetchContent() {
    this.loading = true;
    if (this.videoId) {
      this.youtubeService.getVideo(this.videoId).subscribe({
        next: (res) => {
          this.loading = false;
          this.video = res;
          console.log('Contents:', res);
        },
        error: (error) => {
          this.loading = false;
          console.error('Error fetching contents:', error);
        }
      });
    }
  }

  onConfirm() {
    this.visible = false;
  }
}
