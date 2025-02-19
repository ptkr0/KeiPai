import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TwitchSuccessComponent } from './twitch-success.component';

describe('TwitchSuccessComponent', () => {
  let component: TwitchSuccessComponent;
  let fixture: ComponentFixture<TwitchSuccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TwitchSuccessComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TwitchSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
