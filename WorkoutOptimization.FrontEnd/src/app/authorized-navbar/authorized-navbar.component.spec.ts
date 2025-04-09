import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizedNavbarComponent } from './authorized-navbar.component';

describe('AuthorizedNavbarComponent', () => {
  let component: AuthorizedNavbarComponent;
  let fixture: ComponentFixture<AuthorizedNavbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AuthorizedNavbarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorizedNavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
