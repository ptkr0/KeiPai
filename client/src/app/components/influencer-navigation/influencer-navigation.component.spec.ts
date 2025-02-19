import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfluencerNavigationComponent } from './influencer-navigation.component';

describe('InfluencerNavigationComponent', () => {
  let component: InfluencerNavigationComponent;
  let fixture: ComponentFixture<InfluencerNavigationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfluencerNavigationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfluencerNavigationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
