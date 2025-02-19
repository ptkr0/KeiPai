import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmitContentDialogComponent } from './submit-content-dialog.component';

describe('SubmitContentDialogComponent', () => {
  let component: SubmitContentDialogComponent;
  let fixture: ComponentFixture<SubmitContentDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubmitContentDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubmitContentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
