import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeveloperSettingsComponent } from './developer-settings.component';

describe('DeveloperSettingsComponent', () => {
  let component: DeveloperSettingsComponent;
  let fixture: ComponentFixture<DeveloperSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeveloperSettingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeveloperSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
