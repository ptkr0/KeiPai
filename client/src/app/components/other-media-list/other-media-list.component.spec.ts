import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherMediaListComponent } from './other-media-list.component';

describe('OtherMediaListComponent', () => {
  let component: OtherMediaListComponent;
  let fixture: ComponentFixture<OtherMediaListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OtherMediaListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OtherMediaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
