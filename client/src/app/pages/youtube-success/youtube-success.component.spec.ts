import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YoutubeSuccessComponent } from './youtube-success.component';

describe('YoutubeSuccessComponent', () => {
  let component: YoutubeSuccessComponent;
  let fixture: ComponentFixture<YoutubeSuccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YoutubeSuccessComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(YoutubeSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
