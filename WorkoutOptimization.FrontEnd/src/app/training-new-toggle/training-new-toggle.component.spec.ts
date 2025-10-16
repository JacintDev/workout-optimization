import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainingNewToggleComponent } from './training-new-toggle.component';

describe('TrainingNewToggleComponent', () => {
  let component: TrainingNewToggleComponent;
  let fixture: ComponentFixture<TrainingNewToggleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TrainingNewToggleComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrainingNewToggleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
