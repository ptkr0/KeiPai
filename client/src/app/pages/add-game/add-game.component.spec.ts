import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddGameComponent } from './add-game.component';

describe('AddGameComponent', () => {
  let component: AddGameComponent;
  let fixture: ComponentFixture<AddGameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddGameComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
