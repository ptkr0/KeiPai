import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TwitchStreamComponent } from './twitch-stream.component';

describe('TwitchStreamComponent', () => {
  let component: TwitchStreamComponent;
  let fixture: ComponentFixture<TwitchStreamComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TwitchStreamComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TwitchStreamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
