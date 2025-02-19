import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, FormsModule, AbstractControl } from '@angular/forms';
import { Editor, NgxEditorModule, Toolbar } from 'ngx-editor';
import { Validators } from '@angular/forms';
import {
  FormBuilder,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GameSearchComponent } from "../../components/game-search/game-search.component";
import { TagSelectComponent } from "../../components/tag-select/tag-select.component";
import { GameService } from '../../services/game.service';
import { ActivatedRoute, Router } from '@angular/router';
import { IGameDetail, IUpdateGame } from '../../models/game.model';
import { fileValidator } from '../../validators/file-validator';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { bootstrapTrash } from '@ng-icons/bootstrap-icons';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-game',
  standalone: true,
  imports: [FormsModule, NgxEditorModule, ReactiveFormsModule, CommonModule, GameSearchComponent, TagSelectComponent, NgIconComponent],
  viewProviders: [provideIcons({ bootstrapTrash })],
  templateUrl: './edit-game.component.html',
  styleUrl: './edit-game.component.css'
})
export class EditGameComponent implements OnInit, OnDestroy {

  editor!: Editor;
  toolbar: Toolbar = [
    ['bold', 'italic'],
    ['underline', 'strike'],
    ['code', 'blockquote'],
    ['ordered_list', 'bullet_list'],
    [{ heading: ['h1', 'h2', 'h3', 'h4', 'h5', 'h6'] }],
    ['link', 'image'],
    ['text_color', 'background_color'],
    ['align_left', 'align_center', 'align_right', 'align_justify'],
  ];

  thumbnailUrl: string | null = null;
  descLength: number = 0;
  editGameForm!: FormGroup;
  editCoverForm!: FormGroup;
  addScreenshotForm!: FormGroup;
  game: IGameDetail | null = null;
  id: number;

  gameService = inject(GameService);
  toast = inject(ToastrService);

  router = inject(Router);
  activatedRoute = inject(ActivatedRoute);

  constructor(private fb: FormBuilder) {
    this.id = this.activatedRoute.snapshot.params['id'];
    this.fetchGame();

    this.editGameForm = this.fb.group({
      name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      description: new FormControl('', [Validators.required, Validators.maxLength(5000)]),
      releaseDate: new FormControl('', [Validators.required]),
      youtubeTrailer: new FormControl('', [Validators.maxLength(50), Validators.pattern(/^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+/)]),
      tags: new FormControl([]),
      youtubeTag: new FormControl('', Validators.maxLength(50)),
      twitchCategory: new FormControl(''),
      minimumCPU: new FormControl('', Validators.maxLength(50)),
      minimumRAM: new FormControl('', Validators.maxLength(50)),
      minimumStorage: new FormControl('', Validators.maxLength(50)),
      minimumOS: new FormControl('', Validators.maxLength(50)),
      minimumGPU: new FormControl('', Validators.maxLength(50)),
      pressKit: new FormControl('', [Validators.maxLength(50), Validators.pattern(/^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+\.[a-zA-Z]{2,}(\/[^\s]*)?$/)]), });

    this.editCoverForm = this.fb.group({
      cover: new FormControl(null, [Validators.required, fileValidator()]),
    });

    this.addScreenshotForm = this.fb.group({
      screenshot: new FormControl(null, [Validators.required, fileValidator(), this.maxScreenshotsValidator()]),
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.editCoverForm.patchValue({
      cover: file
    });
    this.editCoverForm.get('cover')!.updateValueAndValidity()
  }

  maxScreenshotsValidator() {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (this.game && this.game.screenshots.length >= 10) {
        return { 'maxScreenshots': { value: control.value } };
      }
      return null;
    };
  }

  onFileChange2(event: any) {
    const file = event.target.files[0];
    this.addScreenshotForm.patchValue({
      screenshot: file
    });
    this.addScreenshotForm.get('screenshot')!.updateValueAndValidity();
  }

  onUrlInput(): void {
    const url = this.editGameForm.get('youtubeTrailer')?.value;
    const videoId = this.extractVideoId(url ?? '');

    if (videoId) {
      this.thumbnailUrl = `https://img.youtube.com/vi/${videoId}/maxresdefault.jpg`;
    } else {
      this.thumbnailUrl = null;
    }
  }

  fetchGame() {
    this.gameService.getGame(this.id).subscribe({
      next: (game: IGameDetail) => {
        this.game = game;
        
        this.editGameForm.patchValue({
          name: game.name,
          description: game.description,
          releaseDate: game.releaseDate,
          youtubeTrailer: game.youtubeTrailer,
          tags: game.tags.map(tag => tag.id),
          youtubeTag: game.youtubeTag,
          twitchCategory: { id: game.twitchTagId, name: game.twitchTagName },
          minimumCPU: game.minimumCPU,
          minimumRAM: game.minimumRAM,
          minimumStorage: game.minimumStorage,
          minimumOS: game.minimumOS,
          minimumGPU: game.minimumGPU,
          pressKit: game.pressKit,
        });
  
        if (this.game?.youtubeTrailer) {
          const videoId = this.extractVideoId(this.game.youtubeTrailer);
          this.thumbnailUrl = `https://img.youtube.com/vi/${videoId}/maxresdefault.jpg`;
        }
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error fetching game info';
        this.toast.error(errorMessage);
        this.router.navigate(['/games']);
      },
    });
  }

  onContentChange(): void {
    this.descLength = this.editGameForm.get('description')?.value?.length ?? 0;
  }

  extractVideoId(url: string): string | null {
    const regex = /(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
    const match = url.match(regex);
    return match ? match[1] : null;
  }

  ngOnInit(): void {
    this.editor = new Editor();
  }

  ngOnDestroy(): void {
    this.editor.destroy();
  }

  onSubmitBasicEdit() {
    if (this.editGameForm.valid) {
      const updateGame: IUpdateGame = this.editGameForm.value;
      updateGame.TwitchTagId = this.editGameForm.get('twitchCategory')?.value.id;
      updateGame.TwitchTagName = this.editGameForm.get('twitchCategory')?.value.name;
      console.log('Update game:', updateGame);
      this.gameService.updateGame(updateGame, this.id).subscribe({
        next: (game) => {
          this.toast.success('Game updated successfully');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error editing game';
          this.toast.error(errorMessage);
        },
      });
    } else {
      this.editGameForm.markAllAsTouched();
    }
  }

  onSubmitCoverEdit() {
    if (this.editCoverForm.valid) {
      const formData = new FormData();
      formData.append('CoverPhoto', this.editCoverForm.get('cover')?.value);
      this.gameService.updateCover(formData, this.id).subscribe({
        next: (game) => {
          this.game!.coverPhoto = game.coverPhoto;
          this.toast.success('Cover updated successfully');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error updating cover';
          this.toast.error(errorMessage);
        },
      });
    } else {
      this.editCoverForm.markAllAsTouched();
    }
  }

  deleteScreenshot(screenshotId: number) {
    this.gameService.deleteScreenshot(screenshotId).subscribe({
      next: () => {
        this.game!.screenshots = this.game!.screenshots.filter(screenshot => screenshot.id !== screenshotId);
        this.toast.success('Screenshot deleted successfully');
      },
      error: (error) => {
        const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error deleting screenshot';
        this.toast.error(errorMessage);
      },
    });
  }

  uploadScreenshot() {
    if (this.addScreenshotForm.valid) {
      const formData = new FormData();
      const formValue = this.addScreenshotForm.value;
      formData.append('screenshots', formValue.screenshot);
      this.gameService.addScreenshot(formData, this.id).subscribe({
        next: (screenshot) => {
          this.toast.success('Screenshot added successfully');
          this.game!.screenshots.push(screenshot.screenshots[0]);
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error adding screenshot';
          this.toast.error(errorMessage);
        },
      });
    }
  }
}
