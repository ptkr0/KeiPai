import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { IConnectOtherMedia } from '../../models/otherMedia.model';

@Component({
  selector: 'app-connect-other-media-dialog',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './connect-other-media-dialog.component.html',
  styleUrl: './connect-other-media-dialog.component.css'
})
export class ConnectOtherMediaDialogComponent {
  @Input() visible = false;
  @Output() confirm = new EventEmitter<IConnectOtherMedia>();
  @Output() cancel = new EventEmitter<void>();

  addOtherMediaForm: FormGroup;
  roles: string[] = ["Streamer", "Influencer", "Blogger", "Reporter", "Reviewer", "Press", "Curator", "Freelance Writer"];
  mediums: string[] = ["Website", "Blog", "Facebook Page", "Twitter Profile", "Instagram Profile", "TV", "Radio", "Podcast", "Steam Group", "Magazine"];

  constructor(private fb: FormBuilder) {
    this.addOtherMediaForm = this.fb.group({
      name: new FormControl('', [Validators.required]),
      url: new FormControl('', [Validators.required, Validators.pattern(/^(https?:\/\/)?(www\.)?[a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$/)]),
      role: new FormControl('', [Validators.required]),
      medium: new FormControl('', [Validators.required]),
      sampleContent: new FormControl('', [Validators.required]),
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && changes['visible'].currentValue === true) {
      this.addOtherMediaForm.reset({
        name: '',
        url: '',
        role: '',
        medium: '',
        sampleContent: '',
      });
    }
  }

  onSubmit() {
    if (this.addOtherMediaForm.valid) {
      const otherMedia: IConnectOtherMedia = this.addOtherMediaForm.value;
      this.confirm.emit(otherMedia);
      this.visible = false;
    }
  }

  onCancel() {
    this.cancel.emit();
    this.visible = false;
  }
}
