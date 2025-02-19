import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteKeysBulkComponent } from './delete-keys-bulk.component';

describe('DeleteKeysBulkComponent', () => {
  let component: DeleteKeysBulkComponent;
  let fixture: ComponentFixture<DeleteKeysBulkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteKeysBulkComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteKeysBulkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
