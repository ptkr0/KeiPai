import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestsAcceptedComponent } from './requests-accepted.component';

describe('RequestsAcceptedComponent', () => {
  let component: RequestsAcceptedComponent;
  let fixture: ComponentFixture<RequestsAcceptedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestsAcceptedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestsAcceptedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
