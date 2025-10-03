import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BicepsCurlAnimateComponent } from './biceps-curl-animate.component';

describe('BicepsCurlAnimateComponent', () => {
  let component: BicepsCurlAnimateComponent;
  let fixture: ComponentFixture<BicepsCurlAnimateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BicepsCurlAnimateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BicepsCurlAnimateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
