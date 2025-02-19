import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageKeysComponent } from './manage-keys.component';

describe('ManageKeysComponent', () => {
  let component: ManageKeysComponent;
  let fixture: ComponentFixture<ManageKeysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageKeysComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageKeysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
