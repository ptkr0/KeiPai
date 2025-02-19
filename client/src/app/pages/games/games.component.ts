import { Component, inject } from '@angular/core';
import { GameComponent } from "../../components/game/game.component";
import { HttpClient } from '@angular/common/http';
import { IGame } from '../../models/game.model';
import { GameService } from '../../services/game.service';
import { UserService } from '../../services/user.service';
import { IPagination } from '../../models/pagination.model';
import { Router } from '@angular/router';
import { DevNavigationComponent } from "../../components/dev-navigation/dev-navigation.component";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-games',
  standalone: true,
  imports: [GameComponent, DevNavigationComponent],
  templateUrl: './games.component.html',
  styleUrl: './games.component.css'
})
export class GamesComponent {

  userService = inject(UserService);
  gameService = inject(GameService);
  toast = inject(ToastrService);

  http: HttpClient = inject(HttpClient);
  router: Router = inject(Router);

  games: IGame[] = [];
  pagination: IPagination | null = null;

  constructor() {
    if(this.userService.user()?.role === "Developer") {
      this.fetchGames();
    }
    else {
        this.router.navigateByUrl('/dashboard');
    }
  }

  fetchGames() {
    if (this.games.length > 0) {
      return;
    }
    this.gameService.getGames(this.userService.user()!.id, 1).subscribe({
      next: (res) => {
        this.pagination = res;
        this.games = res.games;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching games';
        this.toast.error(errorMessage);
      }
    });
  }

  deleteGame(gameId: number) {
    console.log('Deleting game:', gameId);
    this.gameService.deleteGame(gameId).subscribe({
      next: () => {
        this.toast.success('Game deleted successfully');
        this.games = this.games.filter((game) => game.id !== gameId);
        this.pagination!.totalCount -= 1;
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error deleting game';
        this.toast.error(errorMessage);
      }
    });
  }

  blockAddGameButton() : boolean {
    return (this.pagination?.totalCount ?? 0) > 100;
  }

  redirectToAddGame() {
    this.router.navigateByUrl('/add-game');
  }
}
