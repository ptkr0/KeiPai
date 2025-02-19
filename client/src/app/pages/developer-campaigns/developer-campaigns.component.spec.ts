import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeveloperCampaignsComponent } from './developer-campaigns.component';

describe('DeveloperCampaignsComponent', () => {
  let component: DeveloperCampaignsComponent;
  let fixture: ComponentFixture<DeveloperCampaignsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeveloperCampaignsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeveloperCampaignsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
