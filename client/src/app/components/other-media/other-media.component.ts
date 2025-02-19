import { Component, Input } from '@angular/core';
import { IOtherMedia } from '../../models/otherMedia.model';

@Component({
  selector: 'app-other-media',
  standalone: true,
  imports: [],
  templateUrl: './other-media.component.html',
  styleUrl: './other-media.component.css'
})
export class OtherMediaComponent {

  @Input()
  content!: IOtherMedia;
}
