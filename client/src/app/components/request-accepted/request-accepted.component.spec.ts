import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestAcceptedComponent } from './request-accepted.component';

describe('RequestAcceptedComponent', () => {
  let component: RequestAcceptedComponent;
  let fixture: ComponentFixture<RequestAcceptedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestAcceptedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestAcceptedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
