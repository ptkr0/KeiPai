import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeveloperRequestsComponent } from './developer-requests.component';

describe('DeveloperRequestsComponent', () => {
  let component: DeveloperRequestsComponent;
  let fixture: ComponentFixture<DeveloperRequestsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeveloperRequestsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeveloperRequestsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
