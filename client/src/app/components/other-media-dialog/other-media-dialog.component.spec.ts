import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherMediaDialogComponent } from './other-media-dialog.component';

describe('OtherMediaDialogComponent', () => {
  let component: OtherMediaDialogComponent;
  let fixture: ComponentFixture<OtherMediaDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OtherMediaDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OtherMediaDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
