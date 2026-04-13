import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacultyLogin } from './faculty-login';

describe('FacultyLogin', () => {
  let component: FacultyLogin;
  let fixture: ComponentFixture<FacultyLogin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultyLogin],
    }).compileComponents();

    fixture = TestBed.createComponent(FacultyLogin);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
