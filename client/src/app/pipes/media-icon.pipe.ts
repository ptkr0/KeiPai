import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'mediaIcon',
  standalone: true
})
export class MediaIconPipe implements PipeTransform {

  transform(mediaId: string): string {
    switch (mediaId) {
      case 'youtube':
        return 'bootstrapYoutube';
      case 'twitch':
        return 'bootstrapTwitch';
      case 'other':
        return 'bootstrapNewspaper';
      default:
        return 'bootstrapNewspaper';
    }
  }
}
