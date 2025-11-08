import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecommendedPlanComponent } from './recommended-plan.component';

describe('RecommendedPlanComponent', () => {
  let component: RecommendedPlanComponent;
  let fixture: ComponentFixture<RecommendedPlanComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RecommendedPlanComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecommendedPlanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
