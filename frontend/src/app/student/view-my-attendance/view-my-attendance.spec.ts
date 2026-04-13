import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMyAttendance } from './view-my-attendance';

describe('ViewMyAttendance', () => {
  let component: ViewMyAttendance;
  let fixture: ComponentFixture<ViewMyAttendance>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewMyAttendance],
    }).compileComponents();

    fixture = TestBed.createComponent(ViewMyAttendance);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
