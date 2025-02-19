import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { IAddKeys } from '../../models/key.model';
import { PlatformService } from '../../services/platform.service';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  imports: [FormsModule, ReactiveFormsModule],
  standalone: true,
  selector: 'app-add-keys-dialog',
  templateUrl: './add-keys-dialog.component.html',
})
export class AddKeysDialogComponent implements OnChanges {
  @Input() visible = false;
  @Input() gameId!: number;
  @Output() confirm = new EventEmitter<IAddKeys>();
  @Output() cancel = new EventEmitter<void>();

  private platformService = inject(PlatformService);
  platforms = this.platformService.getPlatforms();
  numOfKeys: number = 0;
  addKeysForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.addKeysForm = this.fb.group({
      platform: new FormControl('', [Validators.required]),
      keys: new FormControl('', [Validators.required]),
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && changes['visible'].currentValue === true) {
      this.addKeysForm.reset({
        platform: '',
        keys: '',
      });
      this.numOfKeys = 0;
    }
  }

  onKeysChange(event: any) {
    const keys = event.target.value;
    const lines = keys.split(/\r|\r\n|\n/).filter((line: string) => line.trim() !== '');
    this.numOfKeys = lines.length;
  }

  onSubmit() {
    if (this.addKeysForm.valid) {
      const keys: IAddKeys = {
        gameId: this.gameId,
        platformId: parseInt(this.addKeysForm.get('platform')?.value, 10),
        keys: this.addKeysForm.get('keys')?.value.split(/\r|\r\n|\n/).filter((line: string) => line.trim() !== '').map((line: string) => { return { key: line }; })
      };
      this.confirm.emit(keys);
      this.visible = false;
    }
  }

  onCancel() {
    this.cancel.emit();
    this.visible = false;
  }
}