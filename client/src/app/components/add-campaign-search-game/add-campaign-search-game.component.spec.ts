import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCampaignSearchGameComponent } from './add-campaign-search-game.component';

describe('AddCampaignSearchGameComponent', () => {
  let component: AddCampaignSearchGameComponent;
  let fixture: ComponentFixture<AddCampaignSearchGameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddCampaignSearchGameComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddCampaignSearchGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
