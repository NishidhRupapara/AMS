import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddFaculty } from './add-faculty';

describe('AddFaculty', () => {
  let component: AddFaculty;
  let fixture: ComponentFixture<AddFaculty>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddFaculty],
    }).compileComponents();

    fixture = TestBed.createComponent(AddFaculty);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
