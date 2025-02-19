import { Component, inject, Input, OnInit } from '@angular/core';
import { OtherMediaService } from '../../services/other-media.service';
import { IOtherMedia } from '../../models/otherMedia.model';
import { OtherMediaComponent } from "../other-media/other-media.component";

@Component({
  selector: 'app-other-media-list',
  standalone: true,
  imports: [OtherMediaComponent],
  templateUrl: './other-media-list.component.html',
  styleUrl: './other-media-list.component.css'
})
export class OtherMediaListComponent implements OnInit {
  @Input()
  userId: string | undefined;

  contents: IOtherMedia[] = [];

  otherMediaService = inject(OtherMediaService);

  ngOnInit() {
    this.fetchContent();
  }

  fetchContent() {
    if (this.userId) {
      this.otherMediaService.getContent(this.userId, 1).subscribe({
        next: (res) => {
          this.contents = res;
          console.log('Contents:', res);
        },
        error: (error) => {
          console.error('Error fetching contents:', error);
        }
      });
    }
  }
}
