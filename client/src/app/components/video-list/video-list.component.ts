import { Component, inject, Input, OnInit, Renderer2 } from '@angular/core';
import { YoutubeService } from '../../services/youtube.service'; // Adjust the path as necessary
import { IPagination } from '../../models/pagination.model';
import { IGetYoutubeVideo, IYoutubeVideo } from '../../models/youtube.model';
import { YoutubeVideoComponent } from "../youtube-video/youtube-video.component";

@Component({
  selector: 'app-video-list',
  standalone: true,
  imports: [YoutubeVideoComponent],
  templateUrl: './video-list.component.html',
  styleUrl: './video-list.component.css'
})
export class VideoListComponent implements OnInit {

  @Input() userId: string | undefined;

  pagination: IPagination | null = null;
  videos: IGetYoutubeVideo[] = [];

  youtubeService = inject(YoutubeService);
  renderer = inject(Renderer2);

  ngOnInit() {
    this.fetchVideos();
  }

  fetchVideos(pageNumber: number = 1) {
    if (this.userId) {
      this.youtubeService.getVideos(this.userId, pageNumber).subscribe({
        next: (res) => {
          this.videos = res.videos;
          this.pagination = res;
          console.log('Videos:', res);
        },
        error: (error) => {
          console.error('Error fetching videos:', error);
        }
      });
    }
  }

  prevPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage > 1) {
      this.fetchVideos(this.pagination.currentPage - 1);
      this.scrollToTop();
    }
  }

  nextPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage < this.pagination.totalPages) {
      this.fetchVideos(this.pagination.currentPage + 1);
      this.scrollToTop();
    }
  }

  scrollToTop(): void {
    this.renderer.setProperty(document.documentElement, 'scrollTop', 0);
    this.renderer.setProperty(document.body, 'scrollTop', 0);
  }
}
