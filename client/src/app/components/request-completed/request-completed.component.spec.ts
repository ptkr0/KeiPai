import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestCompletedComponent } from './request-completed.component';

describe('RequestCompletedComponent', () => {
  let component: RequestCompletedComponent;
  let fixture: ComponentFixture<RequestCompletedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestCompletedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestCompletedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
