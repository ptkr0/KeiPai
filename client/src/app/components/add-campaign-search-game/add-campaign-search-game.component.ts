import { Component, EventEmitter, forwardRef, inject, Output } from '@angular/core';
import { GameService } from '../../services/game.service';
import { FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { IAddCampaignGame, IIGDB } from '../../models/game.model';
import { debounceTime, distinctUntilChanged, of, switchMap } from 'rxjs';

@Component({
  selector: 'app-add-campaign-search-game',
  standalone: true,
  imports: [ReactiveFormsModule],
  providers: [],
  templateUrl: './add-campaign-search-game.component.html',
  styleUrl: './add-campaign-search-game.component.css'
})
export class AddCampaignSearchGameComponent {

  gameService = inject(GameService);
  searchControl = new FormControl('');
  games: IAddCampaignGame[] = [];
  showDropdown = false;
  selectedGame: IAddCampaignGame | null = null;

  @Output()
  setGameEvent = new EventEmitter<IAddCampaignGame>();

  ngOnInit(): void {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((searchTerm) => {
          if (searchTerm) {
            return this.gameService.addCampaignGetGames(searchTerm);
          } else {
            return of([]);
          }
        })
      )
      .subscribe((games) => {
        this.games = games;
      });
  }

  selectGame(game: any): void {
    this.selectedGame = game;
    this.searchControl.setValue(game.name, { emitEvent: false });
    this.showDropdown = false;
    this.setGameEvent.emit(game);
  }

  writeValue(value: any): void {
    if (value) {
      this.selectedGame = value;
      this.searchControl.setValue(value, { emitEvent: false });
    } else {
      this.selectedGame = null;
      this.searchControl.setValue('', { emitEvent: false });
    }
  }
}

