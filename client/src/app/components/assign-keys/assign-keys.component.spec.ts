import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignKeysComponent } from './assign-keys.component';

describe('AssignKeysComponent', () => {
  let component: AssignKeysComponent;
  let fixture: ComponentFixture<AssignKeysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignKeysComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignKeysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
