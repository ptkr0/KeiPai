import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TwitchStreamDialogComponent } from './twitch-stream-dialog.component';

describe('TwitchStreamDialogComponent', () => {
  let component: TwitchStreamDialogComponent;
  let fixture: ComponentFixture<TwitchStreamDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TwitchStreamDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TwitchStreamDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
