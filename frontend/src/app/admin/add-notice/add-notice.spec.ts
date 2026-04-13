import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNotice } from './add-notice';

describe('AddNotice', () => {
  let component: AddNotice;
  let fixture: ComponentFixture<AddNotice>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddNotice],
    }).compileComponents();

    fixture = TestBed.createComponent(AddNotice);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
