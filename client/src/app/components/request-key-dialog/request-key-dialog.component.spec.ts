import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestKeyDialogComponent } from './request-key-dialog.component';

describe('RequestKeyDialogComponent', () => {
  let component: RequestKeyDialogComponent;
  let fixture: ComponentFixture<RequestKeyDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestKeyDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestKeyDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
