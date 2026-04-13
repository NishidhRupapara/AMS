import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacultyDetail } from './faculty-detail';

describe('FacultyDetail', () => {
  let component: FacultyDetail;
  let fixture: ComponentFixture<FacultyDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultyDetail],
    }).compileComponents();

    fixture = TestBed.createComponent(FacultyDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
