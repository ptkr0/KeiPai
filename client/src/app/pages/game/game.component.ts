import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IGameDetail } from '../../models/game.model';
import { GameService } from '../../services/game.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { MarkdownComponent, provideMarkdown } from 'ngx-markdown';
import { bootstrapEnvelopeFill, bootstrapNewspaper, bootstrapPersonFill, bootstrapTwitch, bootstrapYoutube } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { ConfirmationDialogService } from '../../components/confirmation-dialog/confirmation-dialog.service';
import { ToastrService } from 'ngx-toastr';

interface MediaItem {
  type: 'image' | 'video';
  source: string;
}

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, MarkdownComponent, NgIconComponent, YouTubePlayerModule],
  providers: [provideMarkdown()],
  viewProviders: [provideIcons({ bootstrapYoutube, bootstrapTwitch, bootstrapNewspaper, bootstrapEnvelopeFill, bootstrapPersonFill })],
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent {

  id: number;
  game: IGameDetail | null = null;
  videoId: string | null = null;
  selectedMedia: MediaItem | null = null;

  confirmationDialogService = inject(ConfirmationDialogService);
  gameService = inject(GameService);
  userService = inject(UserService);
  toast = inject(ToastrService);

  activatedRoute = inject(ActivatedRoute);
  router = inject(Router);

  constructor() {
    this.id = this.activatedRoute.snapshot.params['id'];
    this.fetchGame();
  }

  getVideoCoverUrl(id: string | null): string {
    if (id) {
      return `https://img.youtube.com/vi/${id}/maxresdefault.jpg`;
    }
    return '';
  }

  extractVideoId(url: string): string | null {
    const regex = /(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
    const match = url.match(regex);
    return match ? match[1] : null;
  }

  onMediaClick(media: MediaItem) {
    this.selectedMedia = media;
  }

  fetchGame() {
    this.gameService.getGame(this.id).subscribe({
      next: (game) => {
        this.game = game;
        if (this.game.youtubeTrailer) {
          this.videoId = this.extractVideoId(this.game.youtubeTrailer);
          this.selectedMedia = { type: 'video', source: this.videoId ?? '' };
        } else if (this.game.screenshots && this.game.screenshots.length > 0) {
          this.selectedMedia = { type: 'image', source: this.game.screenshots[0].screenshot };
        }
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching game info';
        this.toast.error(errorMessage);
        this.router.navigate(['/games']);
      }
    });
  }

  async deleteGame() {
    const confirmed = await this.confirmationDialogService.confirm(
      'Delete Game '+ this.game?.name+'?',
      'This action is irreversible.',
      'Delete',
      'Cancel'
    );
    if (confirmed) {
      this.gameService.deleteGame(this.id).subscribe({
        next: () => {
          this.toast.success('Game deleted successfully');
          this.router.navigateByUrl('/games');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error deleting game';
          this.toast.error(errorMessage);
        }
      });
    } else {
    }
  }

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }
}