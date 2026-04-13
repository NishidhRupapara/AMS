import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacultyProfile } from './faculty-profile';

describe('FacultyProfile', () => {
  let component: FacultyProfile;
  let fixture: ComponentFixture<FacultyProfile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultyProfile],
    }).compileComponents();

    fixture = TestBed.createComponent(FacultyProfile);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
