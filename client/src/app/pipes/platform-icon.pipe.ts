import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'platformIcon',
  standalone: true
})
export class PlatformIconPipe implements PipeTransform {

  transform(platformId: number): string {
    switch (platformId) {
      case 1:
        return 'simpleSteam';
      case 2:
        return 'simpleEpicgames'; 
      case 3:
        return 'simpleOrigin';
      case 4:
        return 'simpleUbisoft';
      case 5:
        return 'simpleBattledotnet';
      case 6:
        return 'simpleGogdotcom';
      case 7:
        return 'simpleItchdotio';
      case 8:
        return 'bootstrapXbox';
      case 9:
        return 'bootstrapXbox';
      case 10:
        return 'bootstrapPlaystation';
      case 11:
        return 'bootstrapPlaystation';
      case 12:
        return 'bootstrapNintendoSwitch';
      case 13:
        return 'bootstrapAndroid';
      case 14:
        return 'bootstrapApple';
      case 15:
        return 'bootstrapGlobe';
      default:
        return 'bootstrapGlobe';
    }
  }

}
