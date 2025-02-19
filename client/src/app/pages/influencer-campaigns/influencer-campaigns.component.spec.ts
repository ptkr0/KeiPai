import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfluencerCampaignsComponent } from './influencer-campaigns.component';

describe('InfluencerCampaignsComponent', () => {
  let component: InfluencerCampaignsComponent;
  let fixture: ComponentFixture<InfluencerCampaignsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfluencerCampaignsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfluencerCampaignsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
