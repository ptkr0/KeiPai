import { Component, Input, OnInit } from '@angular/core';
import { IGetTwitchStream } from '../../models/twitch.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-twitch-stream',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './twitch-stream.component.html',
  styleUrl: './twitch-stream.component.css'
})
export class TwitchStreamComponent {
  @Input() stream!: IGetTwitchStream;

  constructor() {
   }

  getStreamDuration() {
    const start = new Date(this.stream.stream.startDate);
    const end = new Date(this.stream.stream.endDate);

    const duration = end.getTime() - start.getTime();
    const hours = Math.floor(duration / (1000 * 60 * 60));
    const minutes = Math.floor((duration % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((duration % (1000 * 60)) / 1000);
    
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  }

  getProperThumbnailUrl() {
    // https://static-cdn.jtvnw.net/cf_vods/dgeft87wbj63p/0c5e2cc33313ac2b750a_ptkr_314941790713_1738165743//thumb/thumb0-%{width}x%{height}.jpg
    return this.stream.stream.thumbnail.replace('%{width}', '250').replace('%{height}', '150');
  }
}
