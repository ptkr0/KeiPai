import { Component, inject, Input, Renderer2 } from '@angular/core';
import { IPagination } from '../../models/pagination.model';
import { IGetTwitchStream } from '../../models/twitch.model';
import { TwitchService } from '../../services/twitch.service';
import { TwitchStreamComponent } from "../twitch-stream/twitch-stream.component";

@Component({
  selector: 'app-stream-list',
  standalone: true,
  imports: [TwitchStreamComponent],
  templateUrl: './stream-list.component.html',
  styleUrl: './stream-list.component.css'
})
export class StreamListComponent {

  @Input() userId: string | undefined;

  pagination: IPagination | null = null;
  streams: IGetTwitchStream[] = [];

  twitchService = inject(TwitchService);
  renderer = inject(Renderer2);

  ngOnInit() {
    this.fetchStreams();
  }

  fetchStreams(pageNumber: number = 1) {
    if (this.userId) {
      this.twitchService.getStreams(this.userId, pageNumber).subscribe({
        next: (res) => {
          this.streams = res.streams;
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
      this.fetchStreams(this.pagination.currentPage - 1);
      this.scrollToTop();
    }
  }

  nextPage() {
    if (this.pagination?.currentPage && this.pagination.currentPage < this.pagination.totalPages) {
      this.fetchStreams(this.pagination.currentPage + 1);
      this.scrollToTop();
    }
  }

  scrollToTop(): void {
    this.renderer.setProperty(document.documentElement, 'scrollTop', 0);
    this.renderer.setProperty(document.body, 'scrollTop', 0);
  }
}
