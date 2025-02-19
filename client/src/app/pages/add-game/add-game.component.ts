import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Editor, NgxEditorModule, Toolbar } from 'ngx-editor';
import { Validators } from '@angular/forms';
import {
  FormBuilder,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GameSearchComponent } from "../../components/game-search/game-search.component";
import { fileValidator } from '../../validators/file-validator';
import { TagSelectComponent } from "../../components/tag-select/tag-select.component";
import { GameService } from '../../services/game.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-game',
  standalone: true,
  imports: [FormsModule, NgxEditorModule, ReactiveFormsModule, CommonModule, GameSearchComponent, GameSearchComponent, TagSelectComponent],
  templateUrl: './add-game.component.html',
  styleUrl: './add-game.component.css'
})
export class AddGameComponent implements OnInit, OnDestroy {

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
  addGameForm!: FormGroup;

  gameService = inject(GameService);
  toast = inject(ToastrService);
  
  router = inject(Router);

  constructor(private fb: FormBuilder) {
    this.addGameForm = this.fb.group({
      name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      editorContent: new FormControl('', [Validators.required, Validators.maxLength(5000)]),
      releaseDate: new FormControl('', [Validators.required]),
      youtubeUrl: new FormControl('', [Validators.maxLength(50), Validators.pattern(/^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+/)]),
      cover: new FormControl(null, [Validators.required, fileValidator()]),
      tags: new FormControl([]),
      youtubeTag: new FormControl('', Validators.maxLength(50)),
      twitchCategory: new FormControl(''),
      minimumCPU: new FormControl('', Validators.maxLength(50)),
      minimumRAM: new FormControl('', Validators.maxLength(50)),
      minimumStorage: new FormControl('', Validators.maxLength(50)),
      minimumOS: new FormControl('', Validators.maxLength(50)),
      minimumGPU: new FormControl('', Validators.maxLength(50)),
      pressKit: new FormControl('', [Validators.maxLength(50), Validators.pattern(/^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+\.[a-zA-Z]{2,}(\/[^\s]*)?$/)]),
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.addGameForm.patchValue({
      cover: file
    });
    this.addGameForm.get('cover')!.updateValueAndValidity(); // Trigger validation
  }

  onUrlInput(): void {
    const url = this.addGameForm.get('youtubeUrl')?.value;
    const videoId = this.extractVideoId(url ?? '');

    if (videoId) {
      this.thumbnailUrl = `https://img.youtube.com/vi/${videoId}/maxresdefault.jpg`;
    } else {
      this.thumbnailUrl = null;
    }
  }

  onContentChange(): void {
    this.descLength = this.addGameForm.get('editorContent')?.value?.length ?? 0;
  }

  private extractVideoId(url: string): string | null {
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

  onSubmit() {
    if (this.addGameForm.valid) {
      const formData = new FormData();
      const formValue = this.addGameForm.value;
      formData.append('Name', formValue.name);
      formData.append('Description', formValue.editorContent);
      formData.append('ReleaseDate', formValue.releaseDate);
      formData.append('YoutubeTrailer', formValue.youtubeUrl);
      formData.append('CoverPhoto', formValue.cover);
      formValue.tags.forEach((tagId: number) => {
        formData.append('Tags', tagId.toString());
      });
      formData.append('YoutubeTag', formValue.youtubeTag);
      if(formValue.twitchCategory) {
      formData.append('TwitchTagId', formValue.twitchCategory.id);
      formData.append('TwitchTagName', formValue.twitchCategory.name);
      }
      formData.append('MinimumCPU', formValue.minimumCPU);
      formData.append('MinimumRAM', formValue.minimumRAM);
      formData.append('MinimumStorage', formValue.minimumStorage);
      formData.append('MinimumOS', formValue.minimumOS);
      formData.append('MinimumGPU', formValue.minimumGPU);
      formData.append('PressKit', formValue.pressKit);
      this.gameService.addGame(formData).subscribe({
        next: (game) => {
          this.toast.success('Game added successfully');
          this.router.navigateByUrl('/games');
        },
        error: (error) => {
          const errorMessage = error.error && error.error.length > 0 ? error.error[0].description : 'Error adding game';
          this.toast.error(errorMessage);
      }
      });
    } else {
      this.addGameForm.markAllAsTouched();
    }
  }
}
