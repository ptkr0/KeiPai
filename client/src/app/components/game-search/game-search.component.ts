import { Component, forwardRef, inject, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  of,
} from 'rxjs';
import { GameService } from '../../services/game.service';
import { IIGDB } from '../../models/game.model';

@Component({
  selector: 'app-game-search',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => GameSearchComponent),
      multi: true,
    }
  ],
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './game-search.component.html',
})
export class GameSearchComponent implements OnInit, ControlValueAccessor {

  gameService = inject(GameService);
  searchControl = new FormControl('');
  games: IIGDB[] = [];
  showDropdown = false;
  selectedGame: IIGDB | null = null;

  onChange = (game: any) => {};
  onTouched = () => {};

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  
  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.searchControl.disable() : this.searchControl.enable();
  }

  ngOnInit(): void {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((searchTerm) => {
          if (searchTerm) {
            return this.gameService.searchKeyword(searchTerm);
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
    this.onChange(game);
    this.showDropdown = false;
  }

  writeValue(value: any): void {
    if (value) {
      this.selectedGame = value;
      this.searchControl.setValue(value.name, { emitEvent: false });
    } else {
      this.selectedGame = null;
      this.searchControl.setValue('', { emitEvent: false });
    }
  }

  onBlur(): void {
    this.onTouched();
    setTimeout(() => {
      this.showDropdown = false;
    }, 200);
  }
}
