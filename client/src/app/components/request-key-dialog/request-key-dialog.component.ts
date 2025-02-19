import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { IAddKeys } from '../../models/key.model';
import { IPlatformCampaign } from '../../models/platform.model';
import { FinalPlatforms } from '../../pages/campaign/campaign.component';
import { ISendRequest } from '../../models/request.model';

@Component({
  selector: 'app-request-key-dialog',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './request-key-dialog.component.html',
  styleUrl: './request-key-dialog.component.css'
})
export class RequestKeyDialogComponent {

  @Input() visible = false;
  @Input() campaignId!: number;
  @Input() platforms?: IPlatformCampaign[];
  @Input() media?: FinalPlatforms;
  @Input() codeDistribution?: boolean;
  @Output() confirm = new EventEmitter<ISendRequest>();
  @Output() cancel = new EventEmitter<void>();

  sendRequest: FormGroup;

  constructor(private fb: FormBuilder) {
    this.sendRequest = this.fb.group({
      platform: new FormControl(0, [Validators.required]),
      media: new FormControl('', [Validators.required]),
      confirm: new FormControl(false, [Validators.requiredTrue]),
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && changes['visible'].currentValue === true) {
      this.sendRequest.reset({
        platform: '',
        media: '',
        confirm: false
      });
    }
  }

  onSubmit() {
    if (this.sendRequest.valid) {
      let request: ISendRequest = { campaignId: this.campaignId, platformId: this.sendRequest.value.platform*1, contentPlatformType: this.sendRequest.value.media };
      this.confirm.emit(request);
      this.visible = false;
    }
  }

  onCancel() {
    this.cancel.emit();
    this.visible = false;
  }
}
