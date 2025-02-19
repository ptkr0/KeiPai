import { Injectable } from '@angular/core';
import { IPlatform } from '../models/platform.model';

@Injectable({
  providedIn: 'root'
})
export class PlatformService {
  platforms: IPlatform[] = [
    { id: 1, name: 'Steam' },
    { id: 2, name: 'Epic Games' },
    { id: 3, name: 'Origin' },
    { id: 4, name: 'Uplay' },
    { id: 5, name: 'Battle.net' },
    { id: 6, name: 'GOG' },
    { id: 7, name: 'itch.io' },
    { id: 8, name: 'Xbox One' },
    { id: 9, name: 'Xbox Series X' },
    { id: 10, name: 'PlayStation 5' },
    { id: 11, name: 'PlayStation 4' },
    { id: 12, name: 'Nintendo Switch' },
    { id: 13, name: 'Android' },
    { id: 14, name: 'iOS' },
    { id: 15, name: 'Other' }
  ];
  constructor() { }

  getPlatforms(): IPlatform[] {
    return this.platforms;
  }

  getPlatform(id: number): IPlatform | undefined {
    return this.platforms.find(platform => platform.id === id);
  }
}
