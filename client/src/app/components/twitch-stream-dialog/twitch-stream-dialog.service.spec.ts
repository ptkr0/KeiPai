import { TestBed } from '@angular/core/testing';

import { TwitchStreamDialogService } from './twitch-stream-dialog.service';

describe('TwitchStreamDialogService', () => {
  let service: TwitchStreamDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TwitchStreamDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
