import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule, FormsModule, FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { PlatformService } from '../../services/platform.service';

@Component({
  selector: 'app-delete-keys-bulk',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './delete-keys-bulk.component.html',
  styleUrls: ['./delete-keys-bulk.component.css']
})
export class DeleteKeysBulkComponent implements OnChanges {
  @Input() visible = false;
  @Input() gameId!: number;
  @Output() confirm = new EventEmitter<number>();
  @Output() cancel = new EventEmitter<void>();

  private platformService = inject(PlatformService);
  platforms = this.platformService.getPlatforms();
  deleteKeysForm!: FormGroup;

  constructor(private fb: FormBuilder) {
    this.deleteKeysForm = this.fb.group({
      platform: new FormControl('', [Validators.required]),
      confirm: new FormControl(false, [Validators.requiredTrue]),
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && changes['visible'].currentValue === true) {
      this.deleteKeysForm.reset({
        platform: '',
        confirm: false
      });
    }
  }

  onConfirm() {
    if (this.deleteKeysForm.valid) {
      this.confirm.emit(this.deleteKeysForm.get('platform')?.value * 1);
    }
  }

  onCancel() {
    this.cancel.emit();
  }
}
