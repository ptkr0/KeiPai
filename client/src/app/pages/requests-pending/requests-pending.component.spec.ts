import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestsPendingComponent } from './requests-pending.component';

describe('RequestsPendingComponent', () => {
  let component: RequestsPendingComponent;
  let fixture: ComponentFixture<RequestsPendingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestsPendingComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestsPendingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
