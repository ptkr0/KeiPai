import { TestBed } from '@angular/core/testing';

import { OtherMediaService } from './other-media.service';

describe('OtherMediaService', () => {
  let service: OtherMediaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OtherMediaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
