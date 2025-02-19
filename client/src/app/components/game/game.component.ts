import { Component, EventEmitter, inject, Inject, Input, Output } from '@angular/core';
import { IGame } from '../../models/game.model';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from '../confirmation-dialog/confirmation-dialog.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [],
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {

  router = inject(Router);
  @Input()
  game!: IGame;
  confirmationDialogService = inject(ConfirmationDialogService);

  userService = inject(UserService);
  
  @Output()
  deleteGameEvent = new EventEmitter<number>();

  async deleteGame(gameId: number) {
    const confirmed = await this.confirmationDialogService.confirm(
      'Delete Game '+ this.game?.name+'?',
      'This action is irreversible.',
      'Delete',
      'Cancel'
    );
    if (confirmed) {
      this.deleteGameEvent.emit(gameId);
    } else {
      console.log('Deletion canceled');
    }
  }

  redirector(id: number, path: string) {
    this.router.navigate([path, id]);
  }
}
