import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAttendance } from './view-attendance';

describe('ViewAttendance', () => {
  let component: ViewAttendance;
  let fixture: ComponentFixture<ViewAttendance>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewAttendance],
    }).compileComponents();

    fixture = TestBed.createComponent(ViewAttendance);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
