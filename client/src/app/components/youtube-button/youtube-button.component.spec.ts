import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YoutubeButtonComponent } from './youtube-button.component';

describe('YoutubeButtonComponent', () => {
  let component: YoutubeButtonComponent;
  let fixture: ComponentFixture<YoutubeButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YoutubeButtonComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(YoutubeButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
