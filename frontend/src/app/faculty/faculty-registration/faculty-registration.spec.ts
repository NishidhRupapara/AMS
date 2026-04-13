import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacultyRegistration } from './faculty-registration';

describe('FacultyRegistration', () => {
  let component: FacultyRegistration;
  let fixture: ComponentFixture<FacultyRegistration>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultyRegistration],
    }).compileComponents();

    fixture = TestBed.createComponent(FacultyRegistration);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
