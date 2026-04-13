import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacultyNotice } from './faculty-notice';

describe('FacultyNotice', () => {
  let component: FacultyNotice;
  let fixture: ComponentFixture<FacultyNotice>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultyNotice],
    }).compileComponents();

    fixture = TestBed.createComponent(FacultyNotice);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
