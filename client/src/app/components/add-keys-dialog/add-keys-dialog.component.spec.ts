import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddKeysDialogComponent } from './add-keys-dialog.component';

describe('AddKeysDialogComponent', () => {
  let component: AddKeysDialogComponent;
  let fixture: ComponentFixture<AddKeysDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddKeysDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddKeysDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
