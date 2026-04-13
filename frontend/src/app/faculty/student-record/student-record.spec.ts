import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentRecord } from './student-record';

describe('StudentRecord', () => {
  let component: StudentRecord;
  let fixture: ComponentFixture<StudentRecord>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentRecord],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentRecord);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
