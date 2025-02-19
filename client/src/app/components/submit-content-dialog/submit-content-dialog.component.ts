import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { fileValidator } from '../../validators/file-validator';
import { IBasicGame } from '../../models/game.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-submit-content-dialog',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './submit-content-dialog.component.html',
  styleUrl: './submit-content-dialog.component.css'
})
export class SubmitContentDialogComponent {
  @Input() visible = false;
  @Input() game: IBasicGame | undefined = undefined;

  @Output() confirm = new EventEmitter<FormData>();
  @Output() cancel = new EventEmitter<void>();

  addContentForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.addContentForm = this.fb.group({
      title: new FormControl('', [Validators.required]),
      description: new FormControl('', [Validators.required]),
      url: new FormControl('', [Validators.required, Validators.pattern(/^(https?:\/\/)?(www\.)?[a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$/)]),
      thumbnail: new FormControl(null, [fileValidator()]),
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && changes['visible'].currentValue === true) {
      this.addContentForm.reset({
        title: '',
        description: '',
        url: '',
        thumbnail: null,
      });
    }
  }

  onCancel() {
    this.cancel.emit();
    this.visible = false;
  }

  onSubmit() {
    if (this.addContentForm.valid) {
      const formData = new FormData();
      const formValue = this.addContentForm.value;

      formData.append('Title', formValue.title);
      formData.append('Description', formValue.description);
      formData.append('Url', formValue.url);
      formData.append('Thumbnail', formValue.thumbnail);
      formData.append('GameIds', this.game?.id?.toString() || '');

      this.confirm.emit(formData);
      this.visible = false;
    }
  }

  onFileSelected(event: Event): void {
    event.preventDefault();

    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.addContentForm.patchValue({ thumbnail: file });
      this.addContentForm.get('thumbnail')?.updateValueAndValidity();
    }
  }
}
