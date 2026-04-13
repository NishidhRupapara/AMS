import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FecultyStudent } from './feculty-student';

describe('FecultyStudent', () => {
  let component: FecultyStudent;
  let fixture: ComponentFixture<FecultyStudent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FecultyStudent],
    }).compileComponents();

    fixture = TestBed.createComponent(FecultyStudent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
