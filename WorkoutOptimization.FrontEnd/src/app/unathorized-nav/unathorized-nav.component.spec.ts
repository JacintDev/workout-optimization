import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnathorizedNavComponent } from './unathorized-nav.component';

describe('UnathorizedNavComponent', () => {
  let component: UnathorizedNavComponent;
  let fixture: ComponentFixture<UnathorizedNavComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UnathorizedNavComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UnathorizedNavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
