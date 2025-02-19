import { Component, Input, OnInit } from '@angular/core';
import { IGetYoutubeVideo } from '../../models/youtube.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-youtube-video-dialog',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './youtube-video.component.html',
  styleUrl: './youtube-video.component.css'
})
export class YoutubeVideoComponent {

  @Input() video!: IGetYoutubeVideo;

  constructor() { }

}
