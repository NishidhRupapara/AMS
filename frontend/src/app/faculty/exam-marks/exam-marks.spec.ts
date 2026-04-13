import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExamMarks } from './exam-marks';

describe('ExamMarks', () => {
  let component: ExamMarks;
  let fixture: ComponentFixture<ExamMarks>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExamMarks],
    }).compileComponents();

    fixture = TestBed.createComponent(ExamMarks);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
