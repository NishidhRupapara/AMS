import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewResults } from './view-results';

describe('ViewResults', () => {
  let component: ViewResults;
  let fixture: ComponentFixture<ViewResults>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewResults],
    }).compileComponents();

    fixture = TestBed.createComponent(ViewResults);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
