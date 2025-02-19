import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectOtherMediaDialogComponent } from './connect-other-media-dialog.component';

describe('ConnectOtherMediaDialogComponent', () => {
  let component: ConnectOtherMediaDialogComponent;
  let fixture: ComponentFixture<ConnectOtherMediaDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConnectOtherMediaDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConnectOtherMediaDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
