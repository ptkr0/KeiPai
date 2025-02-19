import { Component, inject } from '@angular/core';
import { IOtherMedia } from '../../models/otherMedia.model';
import { OtherMediaDialogService } from './other-media-dialog.service';
import { OtherMediaService } from '../../services/other-media.service';

@Component({
  selector: 'app-other-media-dialog',
  standalone: true,
  imports: [],
  templateUrl: './other-media-dialog.component.html',
  styleUrl: './other-media-dialog.component.css'
})
export class OtherMediaDialogComponent {
  visible = false;
  loading = true;
  mediaId: number | undefined = undefined;
  media: IOtherMedia | undefined = undefined;

  otherMediaService = inject(OtherMediaService);

  constructor(private otherMediaDialogService: OtherMediaDialogService) {
    this.otherMediaDialogService.dialogData$.subscribe(data => {
      if (data) {
        this.mediaId = data.mediaId;
        this.visible = true;
        this.fetchContent();
      } else {
        this.visible = false;
      }
    });
  }

  fetchContent() {
    this.loading = true;
    if (this.mediaId) {
      this.otherMediaService.getContentById(this.mediaId).subscribe({
        next: (res) => {
          this.loading = false;
          this.media = res;
          console.log('Contents:', res);
        },
        error: (error) => {
          this.loading = false;
          console.error('Error fetching contents:', error);
        }
      });
    }
  }

  onConfirm() {
    this.visible = false;
  }
}
